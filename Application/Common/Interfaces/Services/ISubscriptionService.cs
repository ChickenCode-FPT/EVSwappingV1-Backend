using Application.Dtos;

namespace Application.Common.Interfaces.Services
{
    public interface ISubscriptionService
    {
        Task<RegisterSubscriptionResponse> RegisterSubscriptionAsync(RegisterSubscriptionRequest request);
        Task<List<SubscriptionPackageDto>> GetAllPackagesAsync();
    }
}
