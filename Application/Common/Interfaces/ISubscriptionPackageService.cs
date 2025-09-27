using Application.SubscriptionPackages.Commands;
using Domain.Dtos;

namespace Application.Common.Interfaces
{
    public interface ISubscriptionPackageService
    {
        Task<List<SubscriptionPackageDto>> GetAllAsync();
        Task<SubscriptionPackageDto> CreateAsync(CreatePackageCommand command);
    }
}
