using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IVehicleRepository
    {
        Task<Vehicle?> GetById(int vehicleId);
        Task<IEnumerable<Vehicle>> GetByUserId(string userId);
        Task Add(Vehicle vehicle);
        Task Update(Vehicle vehicle);
        Task Delete(int vehicleId);
    }
}
