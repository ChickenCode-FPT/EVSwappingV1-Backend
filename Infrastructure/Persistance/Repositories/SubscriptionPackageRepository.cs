using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories
{
    public class SubscriptionPackageRepository //: ISubscriptionPackageRepository
    {
        private readonly EVSwappingV2Context _context;

        public SubscriptionPackageRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<List<SubscriptionPackage>> GetAllAsync()
        {
            return await _context.SubscriptionPackages.ToListAsync();
        }

        public async Task<SubscriptionPackage?> GetByIdAsync(int packageId)
        {
            return await _context.SubscriptionPackages.FirstOrDefaultAsync(p => p.PackageId == packageId);
        }

        public async Task<SubscriptionPackage> AddAsync(SubscriptionPackage package)
        {
            _context.SubscriptionPackages.Add(package);
            await _context.SaveChangesAsync();
            return package;
        }
    }
}
