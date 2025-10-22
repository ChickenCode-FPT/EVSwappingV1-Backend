using Application.Dtos;

namespace Application.Common.Interfaces.Services
{
    public interface IDriverService
    {
        Task<RegisterDriverResponse> RegisterDriver(RegisterDriverRequest request);
    }
}
