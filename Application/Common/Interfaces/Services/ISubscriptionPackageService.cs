using Application.Dtos;
using Application.Dtos.Subscription;

namespace Application.Common.Interfaces.Services
{
    public interface ISubscriptionPackageService
    {
        Task<List<SubscriptionPackageDto>> GetAll();
        Task<SubscriptionPackageDto> Create(CreatePackageRequest request);
        Task Update(int id, UpdatePackageRequest package);
        Task<List<SubscriptionPackageDto>> GetActivePackages();
        Task InactivePackage(int id);
        Task ReactivatePackage(int id);
        Task PublishPackage(int id);
    }
}
