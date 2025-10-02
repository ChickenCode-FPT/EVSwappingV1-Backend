using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder
{
    public class BatterySeeder
    {
        public static async Task SeedAsync(EVSwappingV2Context context)
        {
            await context.Database.MigrateAsync();

            // Seed Battery Models
            if (!context.BatteryModels.Any())
            {
                var models = new List<BatteryModel>
                {
                    new BatteryModel
                    {
                        ModelCode = "BAT-2KWH",
                        Manufacturer = "EVTech",
                        CapacityKwh = 2.0m,
                        Chemistry = "Li-ion",
                        CompatibleVehicleTypes = "Scooter, Bike",
                        CreatedAt = DateTime.UtcNow
                    },
                    new BatteryModel
                    {
                        ModelCode = "BAT-5KWH",
                        Manufacturer = "EVTech",
                        CapacityKwh = 5.0m,
                        Chemistry = "Li-ion",
                        CompatibleVehicleTypes = "Car, Van",
                        CreatedAt = DateTime.UtcNow
                    },
                    new BatteryModel
                    {
                        ModelCode = "BAT-7KWH",
                        Manufacturer = "EVPower",
                        CapacityKwh = 7.0m,
                        Chemistry = "Li-ion",
                        CompatibleVehicleTypes = "SUV",
                        CreatedAt = DateTime.UtcNow
                    },
                    new BatteryModel
                    {
                        ModelCode = "BAT-10KWH",
                        Manufacturer = "GreenVolt",
                        CapacityKwh = 10.0m,
                        Chemistry = "Li-ion",
                        CompatibleVehicleTypes = "Truck, Bus",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.BatteryModels.AddRangeAsync(models);
                await context.SaveChangesAsync();
            }

            // Seed Batteries + Inventories
            if (!context.Batteries.Any())
            {
                var models = await context.BatteryModels.ToListAsync();
                var stations = await context.Stations.ToListAsync();
                var rnd = new Random();
                var batteries = new List<Battery>();
                var inventories = new List<StationInventory>();

                int serialCounter = 1;
                string[] statuses = { "Available", "Charging", "Reserved", "Faulty" };

                foreach (var model in models)
                {
                    for (int i = 0; i < 5; i++) // 5 pin cho mỗi model
                    {
                        var battery = new Battery
                        {
                            SerialNumber = $"BAT{serialCounter:D4}",
                            BatteryModelId = model.BatteryModelId,
                            CurrentSoH = 80 + rnd.Next(0, 20), // 80% - 99%
                            CycleCount = rnd.Next(0, 200),
                            Status = statuses[rnd.Next(statuses.Length)],
                            CreatedAt = DateTime.UtcNow
                        };

                        batteries.Add(battery);
                        serialCounter++;
                    }
                }

                await context.Batteries.AddRangeAsync(batteries);
                await context.SaveChangesAsync();

                // Gán pin vào các Station ngẫu nhiên
                foreach (var battery in batteries)
                {
                    var station = stations[rnd.Next(stations.Count)];
                    inventories.Add(new StationInventory
                    {
                        StationId = station.StationId,
                        BatteryId = battery.BatteryId,
                        SlotNumber = $"S{rnd.Next(1, station.Capacity):D2}",
                        Status = battery.Status,
                        CheckedAt = DateTime.UtcNow
                    });
                }

                await context.StationInventories.AddRangeAsync(inventories);
                await context.SaveChangesAsync();
            }
        }
    }
}
