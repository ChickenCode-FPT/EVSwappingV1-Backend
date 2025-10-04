using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface ISubscriptionPackageRepository
    {
        Task<List<SubscriptionPackage>> GetAll();
        Task<SubscriptionPackage?> GetById(int packageId);
        Task<SubscriptionPackage> Add(SubscriptionPackage package);
    }
}
