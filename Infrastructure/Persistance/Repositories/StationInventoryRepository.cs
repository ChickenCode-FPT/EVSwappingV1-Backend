using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistance.Repositories
{
    public class StationInventoryRepository : IStationInventoryRepository
    {
        private readonly EVSwappingV2Context _context;
        private readonly ILogger<StationInventoryRepository> _logger;

        public StationInventoryRepository(EVSwappingV2Context context, ILogger<StationInventoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<StationInventory?> GetInventory(int stationId, CancellationToken ct)
        {
            return await _context.StationInventories
                .Include(si => si.Battery)
                    .ThenInclude(b => b.BatteryModel)
                .FirstOrDefaultAsync(si => si.StationInventoryId == stationId, ct);
        }

        public async Task<IEnumerable<StationInventory>> GetInventorys(CancellationToken ct)
        {
            return await _context.StationInventories
                .Include(si => si.Battery)
                    .ThenInclude(b => b.BatteryModel)
                .ToListAsync(ct);
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

        public async Task<List<int>> GetFullBatteryIdsByModel(int stationId, int batteryModelId)
        {
            return await _context.StationInventories
                .Where(inv => inv.StationId == stationId &&
                    inv.Status == StationInventoryStatus.Full &&
                    inv.Battery.Status == BatteryStatus.Full &&
                    inv.Battery.BatteryModelId == batteryModelId)
                .Select(inv => inv.BatteryId)
                .Distinct()
                .ToListAsync();
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
                    si.Battery.Status == BatteryStatus.Full &&
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
                throw new InvalidOperationException($"Không tìm thấy pin #{batteryId} trong kho trạm #{stationId}.");

            if (inv.Status != StationInventoryStatus.Full)
                throw new InvalidOperationException($"Pin #{batteryId} hiện không ở trạng thái sẵn sàng để giữ (trạng thái: {inv.Status}).");

            inv.Status = StationInventoryStatus.Held;
            inv.ReservationId = reservationId;

            if (inv.Battery != null)
                inv.Battery.Status = BatteryStatus.Held;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"[MarkHeld] Battery #{batteryId} tại trạm #{stationId} đã được giữ cho Reservation #{reservationId}.");
        }

        public async Task MarkFull(int batteryId, int stationId)
        {
            var inv = await _context.StationInventories
                .Include(si => si.Battery)
                .FirstOrDefaultAsync(si => si.StationId == stationId && si.BatteryId == batteryId);

            if (inv == null)
            {
                _logger.LogWarning($"[MarkFull] Không tìm thấy pin #{batteryId} trong trạm #{stationId}.");
                return;
            }

            if (inv.Status != StationInventoryStatus.Held && inv.Status != StationInventoryStatus.Empty)
            {
                _logger.LogWarning($"[MarkFull] Bỏ qua Battery #{batteryId} (trạng thái hiện tại: {inv.Status}).");
                return;
            }

            inv.Status = StationInventoryStatus.Full;
            inv.ReservationId = null;

            if (inv.Battery != null)
                inv.Battery.Status = BatteryStatus.Full;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"[MarkFull] Battery #{batteryId} tại trạm #{stationId} đã được trả về trạng thái Full.");
        }
    }
}
