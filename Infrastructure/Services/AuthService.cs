using Application.Common.Interfaces.Services;
using Domain.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        public readonly UserManager<User> _userManager;
        public readonly IConfiguration _config;
        private readonly EmailService _emailService;

        public AuthService(UserManager<User> userManager, IConfiguration config, EmailService emailService)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
        }

        public async Task<LoginResponseDto> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            if (user.LockoutEnd > DateTime.UtcNow)
                throw new Exception("User is locked out until " + user.LockoutEnd?.ToString("u"));

            if (!await _userManager.CheckPasswordAsync(user, password))
                throw new Exception("Invalid credentials");
            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return new LoginResponseDto
                {
                    RequiresTwoFactor = true
                };
            }
            var jwtToken = await GenerateJwtToken(user);

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new LoginResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiryTime,
                RequiresTwoFactor = false
            };
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
        public async Task<string> RegisterAsync(string email, string password, string fullName, string phoneNumber)
        {
            var user = new User
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            return await GenerateJwtToken(user);
        }
        public async Task<string> GoogleCallbackAsync(HttpContext httpContext)
        {
            var result = await httpContext.AuthenticateAsync("Cookies");
            if (!result.Succeeded)
                throw new Exception("Google authentication failed.");

            var claims = result.Principal.Claims.ToList();
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                throw new Exception("Email claim not found from Google.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    UserName = email,
                    FullName = name
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception(errors);
                }

                await _userManager.AddToRoleAsync(user, "User");
            }

            var token = await GenerateJwtToken(user);
            return token;
        }
        public async Task<string> GenerateJwtToken(User user)
        {
            var jwtKey = _config["Jwt:Key"];
            var jwtIssuer = _config["Jwt:Issuer"];
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                  new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                  new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                  new Claim("fullname", user.FullName ?? ""),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim("roles", role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtIssuer,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        public async Task GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"http://localhost:4200/reset-password?email={email}&token={Uri.EscapeDataString(token)}";

            var subject = "Reset your password";
            var body = $"<p>Click the link below to reset your password:</p><p><a href='{resetLink}'>Reset Password</a></p>";

            await _emailService.SendEmailAsync(email, subject, body);
        }


        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null)
                throw new Exception("Invalid refresh token");

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new Exception("Refresh token has expired");

            var jwtToken = await GenerateJwtToken(user);

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new LoginResponseDto
            {
                Token = jwtToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiryTime
            };
        }
        public async Task<LoginResponseDto> UpdatePhoneAsync(string email, string phoneNumber)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("User not found.");

            user.PhoneNumber = phoneNumber;

            var jwtToken = await GenerateJwtToken(user);

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new LoginResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiryTime,
                RequiresTwoFactor = false
            };
        }




        //public async Task<bool> RevokeRefreshTokenAsync(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null) throw new Exception("User not found");
        //    user.RefreshToken = null;
        //    user.RefreshTokenExpiryTime = null;
        //    var result = await _userManager.UpdateAsync(user);
        //    return result.Succeeded;

        //}
    }
}
