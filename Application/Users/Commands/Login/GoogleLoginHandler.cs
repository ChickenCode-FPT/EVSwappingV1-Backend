using Application.Common.Interfaces;
using Domain.Dtos;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.Login
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, GoogleLoginResponseDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;

        public GoogleLoginCommandHandler(UserManager<User> userManager, IConfiguration config, IAuthService authService)
        {
            _userManager = userManager;
            _config = config;
            _authService = authService;
        }

        public async Task<GoogleLoginResponseDto> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var httpContext = request.HttpContext;

            // Authenticate external login
            var result = await httpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (!result.Succeeded)
                throw new Exception("Google authentication failed.");

            var claims = result.Principal.Claims.ToList();
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var googleUserId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email))
                throw new Exception("Email claim not found from Google.");
            bool isNewUser = false;

            // Find or create user
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    UserName = email,
                    FullName = name,
                    PhoneNumber = null
                };
               
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception(errors);
                }

                await _userManager.AddToRoleAsync(user, "Customer");
                isNewUser = true;
            }
            var userLogins = await _userManager.GetLoginsAsync(user);
            if (!userLogins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == googleUserId))
            {
                await _userManager.AddLoginAsync(user, new UserLoginInfo(
                    loginProvider: "Google",
                    providerKey: googleUserId,
                    displayName: "Google"
                ));
            }
            // Clear external cookie
            await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);


            var jwtToken = await  _authService.GenerateJwtToken(user);
            var refreshToken = _authService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);


            return new GoogleLoginResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiryTime,
                FullName = user.FullName,
                Email = user.Email,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                RequirePhone = string.IsNullOrEmpty(user.PhoneNumber)
            };
        }

    }
}
