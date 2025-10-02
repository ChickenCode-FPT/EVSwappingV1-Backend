using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface ISubscriptionPackageRepository
    {
        Task<List<SubscriptionPackage>> GetAllAsync();
        Task<SubscriptionPackage?> GetByIdAsync(int packageId);
        Task<SubscriptionPackage> AddAsync(SubscriptionPackage package);
    }
}
