using Domain.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(string email, string password, string fullName, string phoneNumber);
        Task<LoginResponseDto> LoginAsync(string email, string password);
        Task GeneratePasswordResetTokenAsync(string email);
        Task<string> GenerateJwtToken(User user);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
        Task<string> GoogleCallbackAsync(HttpContext httpContext);
        string GenerateRefreshToken();
        Task<LoginResponseDto> UpdatePhoneAsync(string email, string phoneNumber);

    }
}
