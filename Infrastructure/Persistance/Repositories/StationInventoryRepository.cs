using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

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
                .Where(i =>
                    i.StationId == stationId &&
                    i.Status == StationInventoryStatus.Full &&
                    i.Battery.Status == BatteryStatus.Full);

            if (batteryModelId.HasValue)
                query = query.Where(i => i.Battery.BatteryModelId == batteryModelId.Value);

            return await query.ToListAsync();
        }

        public async Task<int> CountAvailableBatteries(int stationId, int? batteryModelId = null)
        {
            var query = _context.StationInventories
                .Include(i => i.Battery)
                .Where(i =>
                    i.StationId == stationId &&
                    i.Status == StationInventoryStatus.Full &&
                    i.Battery.Status == BatteryStatus.Full);

            if (batteryModelId.HasValue)
                query = query.Where(i => i.Battery.BatteryModelId == batteryModelId.Value);

            return await query.CountAsync();
        }

        public async Task<int> CountFullBatteries(int stationId, int batteryModelId)
        {
            return await _context.StationInventories
                .Include(si => si.Battery)
                .CountAsync(si =>
                    si.StationId == stationId &&
                    si.Status == StationInventoryStatus.Full &&
                    si.Battery.BatteryModelId == batteryModelId);
        }

        public async Task<StationInventory?> GetById(int inventoryId)
        {
            return await _context.StationInventories
                .Include(i => i.Battery)
                .FirstOrDefaultAsync(i => i.StationInventoryId == inventoryId);
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

        public async Task<IEnumerable<Battery>> GetFullBatteriesByModel(int stationId, int batteryModelId)
        {
            return await _context.StationInventories
                .Include(si => si.Battery)
                .Where(si =>
                    si.StationId == stationId &&
                    si.Status == StationInventoryStatus.Full &&
                    si.Battery.BatteryModelId == batteryModelId)
                .Select(si => si.Battery)
                .ToListAsync();
        }

        public async Task MarkHeld(int batteryId, int stationId, int? reservationId = null)
        {
            var inv = await _context.StationInventories
                .Include(si => si.Battery)
                .FirstOrDefaultAsync(si => si.StationId == stationId && si.BatteryId == batteryId);

            if (inv == null)
                throw new InvalidOperationException("Ko tìm thấy pin trong kho trạm.");

            if (inv.Status != StationInventoryStatus.Full)
                throw new InvalidOperationException("Pin hiện không ở trạng thái sẵn sàng để giữ.");

            inv.Status = StationInventoryStatus.Held;
            inv.ReservationId = reservationId;

            if (inv.Battery != null)
                inv.Battery.Status = BatteryStatus.Held;

            await _context.SaveChangesAsync();
        }

        public async Task MarkFull(int batteryId, int stationId)
        {
            var inv = await _context.StationInventories
                .Include(si => si.Battery)
                .FirstOrDefaultAsync(si => si.StationId == stationId && si.BatteryId == batteryId);

            if (inv != null)
            {
                inv.Status = StationInventoryStatus.Full;
                inv.ReservationId = null;

                if (inv.Battery != null)
                    inv.Battery.Status = BatteryStatus.Full;

                await _context.SaveChangesAsync();
            }
        }
    }
}
