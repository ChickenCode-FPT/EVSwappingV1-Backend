using Application.Common.Interfaces;
using Application.Users.Commands.Login;
using Application.Users.Commands.Password;
using Application.Users.Commands.RefreshToken;
using Application.Users.Commands.Register;
using Domain.Dtos;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        public readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;

        public AuthController(IMediator mediator, UserManager<User> userManager, IAuthService authService)
        {
            _mediator = mediator;
            _userManager = userManager;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var token = await _mediator.Send(command);
            return Ok(new { Token = token });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var token = await _mediator.Send(command);
            return Ok(new { Token = token });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok(new { Message = "Password reset link has been sent to your email." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok(new { Message = "Password has been reset successfully." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { token = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback)); 
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }


        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            try
            {
                var command = new GoogleLoginCommand(HttpContext);
                var loginResult = await _mediator.Send(command);
                var frontendUrl = "http://localhost:4200/google-callback";
                var redirectUrl = $"{frontendUrl}?token={loginResult.Token}&requirePhone={loginResult.RequirePhone}&email={loginResult.Email}";

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("update-phone")]
        public async Task<IActionResult> UpdatePhone([FromBody] UpdatePhoneRequestDto dto)
        {
            try
            {
                var command = new UpdatePhoneCommand
                {
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber
                };

                var result = await _mediator.Send(command);
                return Ok(new { Token = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("2fa/setup")]
        public async Task<IActionResult> GetAuthenticatorKey([FromQuery] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User not found");

            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var otpauthUrl = $"otpauth://totp/YourApp:{user.Email}?secret={key}&issuer=YourApp";
            return Ok(new { key, otpauthUrl });
        }

        [HttpPost("2fa/enable")]
        public async Task<IActionResult> Enable2FA([FromBody] Enable2FARequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return NotFound("User not found");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                request.Token
            );

            if (!isValid) return BadRequest("Mã 2FA không hợp lệ");

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            return Ok(new { message = "2FA đã được bật thành công" });
        }

        [HttpPost("2fa/verify")]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] Verify2FARequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return NotFound("User not found");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                TokenOptions.DefaultAuthenticatorProvider,
                request.Token
            );

            if (!isValid)
                return BadRequest("Mã 2FA không hợp lệ");

            var jwtToken = await _authService.GenerateJwtToken(user);
            var refreshToken = _authService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return Ok(new Verify2FAResponse
            {
                Token = jwtToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiryTime
            });
        }
        [HttpPost("disable-2fa")]
        public async Task<IActionResult> Disable2FA([FromBody] Disable2FARequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return NotFound("User not found");

            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
                return BadRequest("Cannot disable 2FA");

            await _userManager.UpdateAsync(user);

            return Ok(new { message = "2FA has been disabled successfully" });
        }
    }
}
