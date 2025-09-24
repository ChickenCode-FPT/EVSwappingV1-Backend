using Domain.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(string email, string password, string fullName,string phoneNumber);
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
