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
        private readonly SubscriptionPackageRepository _packageRepo;
        private readonly SubscriptionRepository _subscriptionRepo;
        private readonly PaymentRepository _paymentRepo;

        public DriverService(
            UserManager<User> userManager,
            DriverRepository driverRepo,
            SubscriptionPackageRepository packageRepo,
            SubscriptionRepository subscriptionRepo,
            PaymentRepository paymentRepo)
        {
            _userManager = userManager;
            _driverRepo = driverRepo;
            _packageRepo = packageRepo;
            _subscriptionRepo = subscriptionRepo;
            _paymentRepo = paymentRepo;
        }

        public async Task<RegisterDriverWithPackageResponse> RegisterDriverWithPackageAsync(
            string userId, string preferredPaymentMethod, int packageId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var existingDriver = await _driverRepo.GetByUserIdAsync(user.Id);
            if (existingDriver != null)
                throw new Exception("User already registered as Driver");

            var package = await _packageRepo.GetByIdAsync(packageId);
            if (package == null) throw new Exception("Package not found");

            // 1. Create Driver
            var driver = new Driver
            {
                UserId = user.Id,
                PreferredPaymentMethod = preferredPaymentMethod,
                CreatedAt = DateTime.UtcNow,
                TotalSwaps = 0
            };
            await _driverRepo.AddAsync(driver);

            // 2. Create Subscription (Pending until payment success)
            var subscription = new Subscription
            {
                UserId = user.Id,
                PackageId = package.PackageId,
                StartDate = DateTime.UtcNow,
                Status = "Pending",
                RemainingSwaps = package.IncludedSwaps,
                CreatedAt = DateTime.UtcNow
            };
            await _subscriptionRepo.AddAsync(subscription);

            // 3. Create Payment
            var payment = new Payment
            {
                UserId = user.Id,
                SwapTransactionId = null,
                Amount = package.Price,
                Currency = "VND",
                Method = preferredPaymentMethod,
                Status = "Pending",
                TransactionRef = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };
            await _paymentRepo.AddAsync(payment);

            return new RegisterDriverWithPackageResponse
            {
                UserId = user.Id,
                DriverId = driver.DriverId,
                SubscriptionId = subscription.SubscriptionId,
                PackageName = package.Name,
                Price = package.Price,
                IncludedSwaps = package.IncludedSwaps,
                RemainingSwaps = subscription.RemainingSwaps,
                StartDate = subscription.StartDate,
                Status = subscription.Status
            };
        }
    }
}
