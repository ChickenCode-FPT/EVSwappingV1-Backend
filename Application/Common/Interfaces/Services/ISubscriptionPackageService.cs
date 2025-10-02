using Application.Dtos;

namespace Application.Common.Interfaces.Services
{
    public interface ISubscriptionPackageService
    {
        Task<List<SubscriptionPackageDto>> GetAll();
        Task<SubscriptionPackageDto> Create(CreatePackageRequest request);
    }
}
