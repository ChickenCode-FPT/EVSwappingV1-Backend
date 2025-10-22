using Application.Dtos;

namespace Application.Common.Interfaces.Services
{
    public interface ISubscriptionService
    {
        Task<RegisterSubscriptionResponse> RegisterSubscription(RegisterSubscriptionRequest request);
        Task<List<SubscriptionPackageDto>> GetAllPackages();
    }
}
