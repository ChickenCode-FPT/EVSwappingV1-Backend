using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistance.Repositories
{
    public class BatteryHealthLogsRepository : IBatteryHealthLogsRepository
    {
        private readonly EVSwappingV2Context _context;
        private readonly ILogger<BatteryHealthLog> _logger;


        public BatteryHealthLogsRepository(EVSwappingV2Context context, ILogger<BatteryHealthLog> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<BatteryHealthLog>> GetAllBatteryHealthLogs()
        {
            try
            {
                var logs = await _context.BatteryHealthLogs
                     .AsNoTracking()
                     .Include(x => x.Battery)
                     .OrderBy(x => x.BatteryId)
                     .ThenBy(x => x.RecordedAt)
                     .ToListAsync();
                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving battery health logs");
                throw;
            }
        }

        public async Task<Battery?> GetBySerialNumberAsync(string serialNumber)
        {
            return await _context.Batteries
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);
        }

        public async Task AddAsync(BatteryHealthLog log)
        {
            _context.BatteryHealthLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<BatteryHealthLog?> GetByIdAsync(int id)
        {
            return await _context.BatteryHealthLogs
                .Include(x => x.Battery)
                .FirstOrDefaultAsync(x => x.BatteryHealthLogId == id);
        }

        public async Task UpdateAsync(BatteryHealthLog log)
        {
            _context.BatteryHealthLogs.Update(log);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(BatteryHealthLog log)
        {
            _context.BatteryHealthLogs.Remove(log);
            await _context.SaveChangesAsync();
        }

    }
}
