using Application.Common.Interfaces;
using Application.SubscriptionPackages.Commands;
using Domain.Dtos;
using Domain.Models;
using Infrastructure.Persistance.Repositories;

namespace Infrastructure.Services
{
    public class SubscriptionPackageService : ISubscriptionPackageService
    {
        private readonly SubscriptionPackageRepository _packageRepo;

        public SubscriptionPackageService(SubscriptionPackageRepository packageRepo)
        {
            _packageRepo = packageRepo;
        }

        public async Task<List<SubscriptionPackageDto>> GetAllAsync()
        {
            var packages = await _packageRepo.GetAllAsync();
            return packages.Select(p => new SubscriptionPackageDto
            {
                PackageId = p.PackageId,
                Name = p.Name,
                BillingCycle = p.BillingCycle,
                Price = p.Price,
                IncludedSwaps = p.IncludedSwaps,
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        public async Task<SubscriptionPackageDto> CreateAsync(CreatePackageCommand command)
        {
            var package = new SubscriptionPackage
            {
                Name = command.Name,
                BillingCycle = command.BillingCycle,
                Price = command.Price,
                IncludedSwaps = command.IncludedSwaps,
                CreatedAt = DateTime.UtcNow
            };

            await _packageRepo.AddAsync(package);

            return new SubscriptionPackageDto
            {
                PackageId = package.PackageId,
                Name = package.Name,
                BillingCycle = package.BillingCycle,
                Price = package.Price,
                IncludedSwaps = package.IncludedSwaps,
                CreatedAt = package.CreatedAt
            };
        }
    }
}
