using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces;
using Application.Dtos;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories
{
    public class StationInventoryRepository : IStationInventoryRepository
    public class StationInventoryRepository: IStationInventoryService
    {
        private readonly EVSwappingV2Context _context;
        private readonly EVSwappingV2Context _db;
        public StationInventoryRepository(EVSwappingV2Context db) => _db = db;

        public StationInventoryRepository(EVSwappingV2Context context)
        public async Task<StationInventoryDto> GetInventory(int stationId, CancellationToken ct)
        {
            _context = context;
        }
            var query = await _db.StationInventories
                .Include(si => si.Battery).ThenInclude(b => b.BatteryModel)
                .Where(si => si.StationId == stationId)
                .ToListAsync(ct);

        public async Task<IEnumerable<StationInventory>> GetByStationId(int stationId)
            return new StationInventoryDto
        {
            return await _context.StationInventories
              .Include(i => i.Battery)
              .Where(i => i.StationId == stationId)
              .ToListAsync();
                StationId = stationId,
                FullBatteries = query.Count(x => x.Battery.Status == "Full"),
                ChargingBatteries = query.Count(x => x.Battery.Status == "Charging"),
                MaintenanceBatteries = query.Count(x => x.Battery.Status == "Maintenance"),
                Batteries = query.Select(x => new BatteryDetailDto
                {
                    BatteryId = x.BatteryId,
                    Model = x.Battery.BatteryModel.ModelCode,
                    Capacity = x.Battery.BatteryModel.CapacityKwh,
                    Status = x.Battery.Status
                }).ToList()
            };
        }

        public async Task<IEnumerable<StationInventory>> GetAvailableBatteries(int stationId, int? batteryModelId = null)
        public async Task<IEnumerable<StationInventoryDto>> GetInventories(CancellationToken ct)
        {
            var query = _context.StationInventories
                .Include(i => i.Battery)
                .Where(i => i.StationId == stationId && i.Status == "Available");
            var query = await _db.StationInventories
                .Include(x => x.Battery)
                .ThenInclude(b => b.BatteryModel)
                .ToListAsync(ct);

            if (batteryModelId.HasValue)
                query = query.Where(i => i.Battery.BatteryModelId == batteryModelId.Value);

            return await query.ToListAsync();
        }

        public async Task<StationInventory?> GetById(int inventoryId)
            var stationInventories = query
                .GroupBy(x => x.StationInventoryId)
                .Select(group => new StationInventoryDto
        {
            return await _context.StationInventories.Include(i => i.Battery).FirstOrDefaultAsync(i => i.StationInventoryId == inventoryId);
        }

        public async Task Add(StationInventory inventory)
                    StationId = group.Key,
                    FullBatteries = group.Count(x => x.Battery != null && x.Battery.Status == "Full"),
                    ChargingBatteries = group.Count(x => x.Battery != null && x.Battery.Status == "Charging"),
                    MaintenanceBatteries = group.Count(x => x.Battery != null && x.Battery.Status == "Maintenance"),
                    Batteries = group.Where(x => x.Battery != null)
                        .Select(x => new BatteryDetailDto
        {
            await _context.StationInventories.AddAsync(inventory);
            await _context.SaveChangesAsync();
        }
                            BatteryId = x.BatteryId,
                            Model = x.Battery.BatteryModel?.ModelCode,
                            Capacity = x.Battery.BatteryModel?.CapacityKwh ?? 0,
                            Status = x.Battery.Status
                        })
                        .ToList()
                })
                .ToList();

        public async Task Update(StationInventory inventory)
        {
            _context.StationInventories.Update(inventory);
            await _context.SaveChangesAsync();
            return stationInventories;
        }

        public async Task Delete(int inventoryId)
        {
            var inv = await _context.StationInventories.FindAsync(inventoryId);
            if (inv != null)
        public async Task<int> GetBatteryCountByStatus(string status)
            {
                _context.StationInventories.Remove(inv);
                await _context.SaveChangesAsync();
            }
            return await _db.StationInventories
                .Include(si => si.Battery)
                .Where(si => si.Status == status)
                .CountAsync();
        }
    }
}
