using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IBatteryRepository
    {
        Task SaveChanges();
        Task<IEnumerable<Battery>> GetInUseByUser(string userId);
        Task<Battery?> GetById(int batteryId);
        Task<IEnumerable<Battery>> GetAvailableBatteries(int? batteryModelId = null);
        Task<int> GetBatteryLast();
        Task<Battery?> GetBySerialNumber(string serialNumber);
        Task UpdateStatus(int batteryId, string status);
        Task Add(Battery battery);
        Task Update(Battery battery);
        Task<List<Battery>> GetAll();
        Task<IEnumerable<Battery>> GetIncomingCandidates(int batteryModelId);
    }
}
