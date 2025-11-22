using Application.Dtos.Battery;
using Domain.Models;

namespace Application.Common.Interfaces.Services
{
    public interface IBatteryService
    {
        Task<IEnumerable<BatteryDto>> GetIncomingCandidates(int batteryModelId);
        Task<IEnumerable<BatteryDto>> GetIncomingForUser(string userId);
        Task<IEnumerable<BatteryDto>> GetAvailableBatteries(int stationId, int? batteryModelId = null);
        Task<IEnumerable<BatteryDto>> GetAvailableOutgoing(int stationId, int? batteryModelId);
        Task<Battery?> GetById(int id);
        Task<List<Battery>> GetAll();
        Task<List<Battery>> GetByStatus(string status);
        Task Add(Battery battery);
        Task Update(Battery battery);
        Task Delete(int id);
    }
}
