using Application.Common.Interfaces;
using Application.Dtos.Battery;
using Application.Dtos.Station;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class StationInventoryService : IStationInventoryService
    {
        private readonly EVSwappingV2Context _db;
        public StationInventoryService(EVSwappingV2Context db) => _db = db;

        public async Task<StationInventoryDto> GetInventory(int stationId, CancellationToken ct)
        {
            var query = await _db.StationInventories
                .Include(si => si.Battery).ThenInclude(b => b.BatteryModel)
                .Where(si => si.StationId == stationId)
                .ToListAsync(ct);

            return new StationInventoryDto
            {
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

        public async Task<IEnumerable<StationInventoryDto>> GetInventories(CancellationToken ct)
        {
            var query = await _db.StationInventories
                .Include(x => x.Battery)
                .ThenInclude(b => b.BatteryModel)
                .ToListAsync(ct);

            var stationInventories = query
                .GroupBy(x => x.StationInventoryId)
                .Select(group => new StationInventoryDto
                {
                    StationId = group.Key,
                    FullBatteries = group.Count(x => x.Battery != null && x.Battery.Status == "Full"),
                    ChargingBatteries = group.Count(x => x.Battery != null && x.Battery.Status == "Charging"),
                    MaintenanceBatteries = group.Count(x => x.Battery != null && x.Battery.Status == "Maintenance"),
                    Batteries = group.Where(x => x.Battery != null)
                        .Select(x => new BatteryDetailDto
                        {
                            BatteryId = x.BatteryId,
                            Model = x.Battery.BatteryModel?.ModelCode,
                            Capacity = x.Battery.BatteryModel?.CapacityKwh ?? 0,
                            Status = x.Battery.Status
                        })
                        .ToList()
                })
                .ToList();

            return stationInventories;
        }

    }
}
