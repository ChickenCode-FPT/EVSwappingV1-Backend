using Application.Dtos;
using Domain.Models;

namespace Application.Common.Interfaces.Services
{
    public interface IBatteryService
    {
        Task<IEnumerable<BatteryDto>> GetAvailableBatteries(int stationId, int? batteryModelId = null);

        Task<Battery?> GetById(int id);
        Task<List<Battery>> GetAll();
        Task<List<Battery>> GetByStatus(string status);
        Task Add(Battery battery);
        Task Update(Battery battery);
        Task Delete(int id);
    }
}
