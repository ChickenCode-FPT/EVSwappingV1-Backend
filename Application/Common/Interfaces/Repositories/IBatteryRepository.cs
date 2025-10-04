using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IBatteryRepository
    {
        Task<Battery?> GetById(int batteryId);
        Task<IEnumerable<Battery>> GetAvailableBatteries(int? batteryModelId = null);
        Task UpdateStatus(int batteryId, string status);
        Task Add(Battery battery);
        Task Update(Battery battery);
    }
}
