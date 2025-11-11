using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IBatteryHealthLogsRepository
    {
        Task<List<BatteryHealthLog>> GetAllBatteryHealthLogs();
        Task<Battery?> GetBySerialNumberAsync(string serialNumber);
        Task<BatteryHealthLog?> GetByIdAsync(int id);
        Task AddAsync(BatteryHealthLog log);
        Task UpdateAsync(BatteryHealthLog log);
        Task DeleteAsync(BatteryHealthLog log);

    }
}
