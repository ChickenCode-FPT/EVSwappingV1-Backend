using Application.Dtos.User;

namespace Application.Common.Interfaces.Services
{
    public interface IVehicleService
    {
        Task<VehicleDto?> GetById(int vehicleId);
        Task<IEnumerable<VehicleDto>> GetByUser();
        Task<VehicleDto> Create(CreateVehicleRequest request);
        Task<VehicleDto> Update(UpdateVehicleRequest request);
        Task Delete(int vehicleId);
    }
}
