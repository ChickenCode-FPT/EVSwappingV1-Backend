using Domain.Dtos;

namespace Application.Common.Interfaces
{
    public interface ISubscriptionService
    {
        Task<RegisterSubscriptionResponse> RegisterSubscriptionAsync(string userId, int packageId);
        Task<List<SubscriptionPackageDto>> GetAllPackagesAsync();
    }
}
