using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;

namespace Infrastructure.Seeder
{
    public class BatterySeeder
    {
        public static async Task SeedAsync(EVSwappingV2Context context)
        {
            await context.Database.MigrateAsync();

            // 1) Seed Battery Models
            if (!context.BatteryModels.Any())
            {
                var models = new List<BatteryModel>
                {
                    new BatteryModel { ModelCode = "BAT-2KWH",  Manufacturer = "EVTech",   CapacityKwh = 2.0m,  Chemistry = "Li-ion", CompatibleVehicleTypes = "Scooter, Bike", CreatedAt = DateTime.UtcNow },
                    new BatteryModel { ModelCode = "BAT-5KWH",  Manufacturer = "EVTech",   CapacityKwh = 5.0m,  Chemistry = "Li-ion", CompatibleVehicleTypes = "Car, Van",        CreatedAt = DateTime.UtcNow },
                    new BatteryModel { ModelCode = "BAT-7KWH",  Manufacturer = "EVPower",  CapacityKwh = 7.0m,  Chemistry = "Li-ion", CompatibleVehicleTypes = "SUV",              CreatedAt = DateTime.UtcNow },
                    new BatteryModel { ModelCode = "BAT-10KWH", Manufacturer = "GreenVolt",CapacityKwh = 10.0m, Chemistry = "Li-ion", CompatibleVehicleTypes = "Truck, Bus",       CreatedAt = DateTime.UtcNow },
                };

                await context.BatteryModels.AddRangeAsync(models);
                await context.SaveChangesAsync();
            }

            // 2) Seed Batteries + StationInventories
            if (!context.Batteries.Any())
            {
                var models = await context.BatteryModels.AsNoTracking().ToListAsync();
                var stations = await context.Stations.AsNoTracking().ToListAsync();
                if (stations.Count == 0)
                {
                    // đảm bảo StationSeeder chạy trước
                    throw new InvalidOperationException("Chưa có Station. Hãy chạy StationSeeder trước BatterySeeder.");
                }

                var rnd = new Random();
                var batteries = new List<Battery>();
                var inventories = new List<StationInventory>();

                int serialCounter = 1;

                // số pin tạo cho mỗi model (có thể chỉnh)
                const int batteriesPerModel = 20;

                // hàm bốc trạng thái với trọng số (đa số Full để “có hàng”)
                string PickBatteryStatus()
                {
                    var roll = rnd.NextDouble(); // 0..1
                    if (roll < 0.60) return BatteryStatus.Full;     // 60%
                    if (roll < 0.75) return BatteryStatus.Empty;    // 15%
                    if (roll < 0.90) return BatteryStatus.InUse;    // 15%
                    return BatteryStatus.Held;                      // 10%
                }

                // 2.1 Tạo Batteries
                foreach (var model in models)
                {
                    for (int i = 0; i < batteriesPerModel; i++)
                    {
                        var st = PickBatteryStatus();
                        var soh = 80 + rnd.Next(0, 20); // 80..99
                        var battery = new Battery
                        {
                            SerialNumber = $"BAT{serialCounter:D4}",
                            BatteryModelId = model.BatteryModelId,
                            CurrentSoH = soh,
                            CycleCount = rnd.Next(0, 500),
                            Status = st,
                            CreatedAt = DateTime.UtcNow
                        };
                        batteries.Add(battery);
                        serialCounter++;
                    }
                }

                await context.Batteries.AddRangeAsync(batteries);
                await context.SaveChangesAsync();

                // 2.2 Phân bổ pin vào StationInventories (không vượt capacity, slot không trùng)
                // tạo map slot đã dùng theo station
                var usedSlotsByStation = stations.ToDictionary(s => s.StationId, _ => new HashSet<string>());

                // helper: map Battery.Status -> StationInventory.Status
                string MapToInventoryStatus(string batteryStatus) => batteryStatus switch
                {
                    BatteryStatus.Full => StationInventoryStatus.Full,
                    BatteryStatus.Held => StationInventoryStatus.Held,
                    // InUse & Empty coi là "không sẵn trong slot", biểu diễn là Empty
                    BatteryStatus.InUse => StationInventoryStatus.Empty,
                    BatteryStatus.Empty => StationInventoryStatus.Empty,
                    _ => StationInventoryStatus.Empty
                };

                // phân đều pin qua các station theo vòng tròn, tôn trọng capacity
                int stationIndex = 0;
                foreach (var battery in batteries)
                {
                    // chọn station tiếp theo còn slot
                    for (int attempts = 0; attempts < stations.Count; attempts++)
                    {
                        var st = stations[stationIndex % stations.Count];
                        stationIndex++;

                        // đếm số inventory đã dùng của station
                        int currentCount = inventories.Count(x => x.StationId == st.StationId);
                        if (currentCount >= st.Capacity) continue; // đầy, thử station khác

                        // random slot không trùng trong [1..Capacity]
                        string slot;
                        var used = usedSlotsByStation[st.StationId];
                        int guard = 0;
                        do
                        {
                            guard++;
                            var n = rnd.Next(1, st.Capacity + 1); // inclusive
                            slot = $"S{n:D2}";
                            if (guard > st.Capacity * 2) break; // tránh vòng vô hạn
                        }
                        while (used.Contains(slot));

                        used.Add(slot);

                        inventories.Add(new StationInventory
                        {
                            StationId = st.StationId,
                            BatteryId = battery.BatteryId,
                            SlotNumber = slot,
                            Status = MapToInventoryStatus(battery.Status),
                            CheckedAt = DateTime.UtcNow,
                            ReservationId = null
                        });

                        break; // gán xong cho 1 station
                    }
                }

                await context.StationInventories.AddRangeAsync(inventories);
                await context.SaveChangesAsync();
            }
        }
    }
}
