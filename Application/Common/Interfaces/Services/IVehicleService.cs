using Application.Dtos;
using Application.Dtos.Requests;

namespace Application.Common.Interfaces.Services
{
    public interface IVehicleService
    {
        Task<VehicleDto?> GetById(int vehicleId);
        Task<IEnumerable<VehicleDto>> GetByUser(string userId);
        Task<VehicleDto> Create(CreateVehicleRequest request);
        Task<VehicleDto> Update(UpdateVehicleRequest request);
        Task Delete(int vehicleId);
    }
}
