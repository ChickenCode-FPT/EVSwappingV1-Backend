using Application.Dtos;

namespace Application.Common.Interfaces.Services
{
    public interface ISubscriptionPackageService
    {
        Task<List<SubscriptionPackageDto>> GetAllAsync();
        Task<SubscriptionPackageDto> CreateAsync(CreatePackageRequest request);
    }
}
