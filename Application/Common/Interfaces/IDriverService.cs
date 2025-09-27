using Domain.Dtos;

namespace Application.Common.Interfaces
{
    public interface IDriverService
    {
        Task<RegisterDriverResponse> RegisterDriverAsync(string userId, string preferredPaymentMethod);
    }
}
