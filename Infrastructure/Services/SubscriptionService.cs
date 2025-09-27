using Application.Common.Interfaces;
using Domain.Dtos;
using Domain.Models;
using Infrastructure.Persistance.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly UserManager<User> _userManager;
        private readonly SubscriptionRepository _subscriptionRepo;
        private readonly SubscriptionPackageRepository _packageRepo;

        public SubscriptionService(
            UserManager<User> userManager,
            SubscriptionRepository subscriptionRepo,
            SubscriptionPackageRepository packageRepo)
        {
            _userManager = userManager;
            _subscriptionRepo = subscriptionRepo;
            _packageRepo = packageRepo;
        }

        public async Task<RegisterSubscriptionResponse> RegisterSubscriptionAsync(string userId, int packageId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var package = await _packageRepo.GetByIdAsync(packageId);
            if (package == null) throw new Exception("Package not found");

            var existing = await _subscriptionRepo.GetActiveByUserIdAsync(user.Id);
            if (existing != null)
                throw new Exception("User already has an active subscription");

            var subscription = new Subscription
            {
                UserId = user.Id,
                PackageId = package.PackageId,
                StartDate = DateTime.UtcNow,
                Status = "Active",
                RemainingSwaps = package.IncludedSwaps,
                CreatedAt = DateTime.UtcNow
            };

            await _subscriptionRepo.AddAsync(subscription);

            return new RegisterSubscriptionResponse
            {
                SubscriptionId = subscription.SubscriptionId,
                UserId = subscription.UserId,
                PackageId = subscription.PackageId,
                PackageName = package.Name,
                Price = package.Price,
                IncludedSwaps = package.IncludedSwaps,
                RemainingSwaps = subscription.RemainingSwaps,
                Status = subscription.Status,
                StartDate = subscription.StartDate
            };
        }

        public async Task<List<SubscriptionPackageDto>> GetAllPackagesAsync()
        {
            var packages = await _packageRepo.GetAllAsync();
            return packages.Select(p => new SubscriptionPackageDto
            {
                PackageId = p.PackageId,
                Name = p.Name,
                BillingCycle = p.BillingCycle,
                Price = p.Price,
                IncludedSwaps = p.IncludedSwaps
            }).ToList();
        }
    }
}
