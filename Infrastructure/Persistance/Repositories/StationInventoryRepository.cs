using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Persistance.Repositories
{
    public class StationInventoryRepository : IStationInventoryRepository
    {
        private readonly EVSwappingV2Context _context;

        public StationInventoryRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StationInventory>> GetByStationId(int stationId)
        {
            return await _context.StationInventories
              .Include(i => i.Battery)
              .Where(i => i.StationId == stationId)
              .ToListAsync();
        }

        public async Task<IEnumerable<StationInventory>> GetAvailableBatteries(int stationId, int? batteryModelId = null)
        {
            var query = _context.StationInventories
                .Include(i => i.Battery)
                .Where(i => i.StationId == stationId && i.Status == "Available");

            if (batteryModelId.HasValue)
                query = query.Where(i => i.Battery.BatteryModelId == batteryModelId.Value);

            return await query.ToListAsync();
        }

        public async Task<StationInventory?> GetById(int inventoryId)
        {
            return await _context.StationInventories.Include(i => i.Battery).FirstOrDefaultAsync(i => i.StationInventoryId == inventoryId);
        }

        public async Task Add(StationInventory inventory)
        {
            await _context.StationInventories.AddAsync(inventory);
            await _context.SaveChangesAsync();
        }

        public async Task Update(StationInventory inventory)
        {
            _context.StationInventories.Update(inventory);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int inventoryId)
        {
            var inv = await _context.StationInventories.FindAsync(inventoryId);
            if (inv != null)
            {
                _context.StationInventories.Remove(inv);
                await _context.SaveChangesAsync();
            }
        }
    }
}
