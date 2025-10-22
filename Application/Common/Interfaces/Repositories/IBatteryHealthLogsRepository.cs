using Application.Dtos;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
