using Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Common.Interfaces.Repositories
{
    public interface ISubscriptionPackageRepository
    {
        Task<SubscriptionPackage> Add(SubscriptionPackage package);
        Task Update(SubscriptionPackage package);
        Task Delete(int packageId);
        Task SaveChanges();
        Task<List<SubscriptionPackage>> GetAll();
        Task<SubscriptionPackage?> GetById(int packageId);
        Task<IEnumerable<SubscriptionPackage>> GetByFilter(string? name = null, string? billingCycle = null);
        Task<IEnumerable<SubscriptionPackage>> GetByPriceRange(decimal min, decimal max);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<IEnumerable<SubscriptionPackage>> GetActivePackages();
        Task InactivePackage(int packageId);
        Task ReactivatePackage(int packageId);
        Task PublishPackage(int packageId);
    }
}
