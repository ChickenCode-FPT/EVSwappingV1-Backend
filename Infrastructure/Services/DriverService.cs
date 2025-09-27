using Application.Common.Interfaces;
using Domain.Dtos;
using Domain.Models;
using Infrastructure.Persistance.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class DriverService : IDriverService
    {
        private readonly UserManager<User> _userManager;
        private readonly DriverRepository _driverRepo;

        public DriverService(UserManager<User> userManager, DriverRepository driverRepo)
        {
            _userManager = userManager;
            _driverRepo = driverRepo;
        }

        public async Task<RegisterDriverResponse> RegisterDriverAsync(string userId, string preferredPaymentMethod)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var existingDriver = await _driverRepo.GetByUserIdAsync(user.Id);
            if (existingDriver != null)
                throw new Exception("User already registered as Driver");

            // 👉 Chỉ tạo Driver, chưa tạo Subscription/Payment
            var driver = new Driver
            {
                UserId = user.Id,
                PreferredPaymentMethod = preferredPaymentMethod,
                CreatedAt = DateTime.UtcNow,
                TotalSwaps = 0
            };
            await _driverRepo.AddAsync(driver);

            return new RegisterDriverResponse
            {
                UserId = user.Id,
                DriverId = driver.DriverId,
                PreferredPaymentMethod = driver.PreferredPaymentMethod,
                CreatedAt = driver.CreatedAt
            };
        }
    }
}
