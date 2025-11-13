using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistance.Repositories
{
    public class SubscriptionPackageRepository : ISubscriptionPackageRepository
    {
        private readonly EVSwappingV2Context _context;

        public SubscriptionPackageRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<SubscriptionPackage> Add(SubscriptionPackage package)
        {
            await _context.SubscriptionPackages.AddAsync(package);
            await _context.SaveChangesAsync();
            return package;
        }

        public async Task Update(SubscriptionPackage package)
        {
            _context.SubscriptionPackages.Update(package);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int packageId)
        {
            var pkg = await _context.SubscriptionPackages.FindAsync(packageId);
            if (pkg == null)
                throw new KeyNotFoundException($"Package with id={packageId} not found.");

            _context.SubscriptionPackages.Remove(pkg);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChanges() => await _context.SaveChangesAsync();

        public async Task<List<SubscriptionPackage>> GetAll()
        {
            return await _context.SubscriptionPackages
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<SubscriptionPackage?> GetById(int packageId)
        {
            return await _context.SubscriptionPackages
                .Include(p => p.Subscriptions)
                .FirstOrDefaultAsync(p => p.PackageId == packageId);
        }

        public async Task<IEnumerable<SubscriptionPackage>> GetByFilter(string? name = null, string? billingCycle = null)
        {
            var query = _context.SubscriptionPackages.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(p => p.Name.Contains(name));

            if (!string.IsNullOrWhiteSpace(billingCycle))
                query = query.Where(p => p.BillingCycle == billingCycle);

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SubscriptionPackage>> GetByPriceRange(decimal min, decimal max)
        {
            return await _context.SubscriptionPackages
                .Where(p => p.Price >= min && p.Price <= max)
                .OrderBy(p => p.Price)
                .ToListAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        }

        public async Task<IEnumerable<SubscriptionPackage>> GetActivePackages()
        {
            return await _context.SubscriptionPackages
                .Where(p => p.Status == "Active")
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task InactivePackage(int packageId)
        {
            var pkg = await _context.SubscriptionPackages.FindAsync(packageId);
            if (pkg == null)
                throw new KeyNotFoundException($"Package with id={packageId} not found.");

            if (pkg.Status != "Active")
                throw new InvalidOperationException("Only active packages can be set to inactive.");

            pkg.Status = "Inactive";
            _context.SubscriptionPackages.Update(pkg);
            await _context.SaveChangesAsync();
        }

        public async Task ReactivatePackage(int packageId)
        {
            var pkg = await _context.SubscriptionPackages.FindAsync(packageId);
            if (pkg == null)
                throw new KeyNotFoundException($"Package with id={packageId} not found.");

            if (pkg.Status != "Inactive")
                throw new InvalidOperationException("Only inactive packages can be reactivated.");

            pkg.Status = "Active";
            _context.SubscriptionPackages.Update(pkg);
            await _context.SaveChangesAsync();
        }
        public async Task PublishPackage(int packageId)
        {
            var pkg = await _context.SubscriptionPackages.FindAsync(packageId);
            if (pkg == null)
                throw new KeyNotFoundException($"Package with id={packageId} not found.");

            if (pkg.Status != "Draft")
                throw new InvalidOperationException("Only draft packages can be published.");

            pkg.Status = "Active";
            _context.SubscriptionPackages.Update(pkg);
            await _context.SaveChangesAsync();
        }


    }
}
