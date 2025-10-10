using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class BatteryRepository : IBatteryRepository
    {
        private readonly EVSwappingV2Context _context;

        public BatteryRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<Battery?> GetById(int batteryId)
        {
            return await _context.Batteries.FindAsync(batteryId);
        }

        public async Task<IEnumerable<Battery>> GetAvailableBatteries(int? batteryModelId = null)
        {
            var query = _context.Batteries.Where(b => b.Status == BatteryStatus.Full);

            if (batteryModelId.HasValue)
                query = query.Where(b => b.BatteryModelId == batteryModelId.Value);

            return await query.ToListAsync();
        }

        public async Task UpdateStatus(int batteryId, string status)
        {
            var battery = await _context.Batteries.FindAsync(batteryId);
            if (battery != null)
            {
                battery.Status = status;
                _context.Batteries.Update(battery);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Add(Battery battery)
        {
            await _context.Batteries.AddAsync(battery);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Battery battery)
        {
            _context.Batteries.Update(battery);
            await _context.SaveChangesAsync();
        }
    }
}
