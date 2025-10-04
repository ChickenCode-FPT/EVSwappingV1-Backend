using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class SubscriptionPackageRepository : ISubscriptionPackageRepository
    {
        private readonly EVSwappingV2Context _context;

        public SubscriptionPackageRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<List<SubscriptionPackage>> GetAll()
        {
            return await _context.SubscriptionPackages.ToListAsync();
        }

        public async Task<SubscriptionPackage?> GetById(int packageId)
        {
            return await _context.SubscriptionPackages.FirstOrDefaultAsync(p => p.PackageId == packageId);
        }

        public async Task<SubscriptionPackage> Add(SubscriptionPackage package)
        {
            _context.SubscriptionPackages.Add(package);
            await _context.SaveChangesAsync();
            return package;
        }
    }
}
