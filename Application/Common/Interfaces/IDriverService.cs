using Domain.Dtos;

namespace Application.Common.Interfaces
{
    public interface IDriverService
    {
        Task<RegisterDriverWithPackageResponse> RegisterDriverWithPackageAsync(string userId, string preferredPaymentMethod, int packageId);
    }
}
