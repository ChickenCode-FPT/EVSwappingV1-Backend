using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder
{
    public class StationSeeder
    {
        public static async Task SeedAsync(EVSwappingV2Context context)
        {
            await context.Database.MigrateAsync();

            if (!context.Stations.Any())
            {
                var stations = new List<Station>
                {
                    new Station
                    {
                        Code = "ST01",
                        Name = "EVSwap Nguyễn Huệ",
                        Address = "123 Nguyễn Huệ, Quận 1, TP.HCM",
                        Latitude = 10.776889m,
                        Longitude = 106.700424m,
                        Capacity = 20,
                        Phone = "0909000001",
                        Status = 1,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Station
                    {
                        Code = "ST02",
                        Name = "EVSwap Điện Biên Phủ",
                        Address = "456 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM",
                        Latitude = 10.801889m,
                        Longitude = 106.715424m,
                        Capacity = 15,
                        Phone = "0909000002",
                        Status = 1,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Station
                    {
                        Code = "ST03",
                        Name = "EVSwap Phạm Văn Đồng",
                        Address = "789 Phạm Văn Đồng, Quận Thủ Đức, TP.HCM",
                        Latitude = 10.852345m,
                        Longitude = 106.730123m,
                        Capacity = 25,
                        Phone = "0909000003",
                        Status = 1,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Station
                    {
                        Code = "ST04",
                        Name = "EVSwap CMT8",
                        Address = "101 Cách Mạng Tháng 8, Quận 10, TP.HCM",
                        Latitude = 10.776123m,
                        Longitude = 106.668912m,
                        Capacity = 18,
                        Phone = "0909000004",
                        Status = 1,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Station
                    {
                        Code = "ST05",
                        Name = "EVSwap Quang Trung",
                        Address = "202 Quang Trung, Quận Gò Vấp, TP.HCM",
                        Latitude = 10.835567m,
                        Longitude = 106.664789m,
                        Capacity = 22,
                        Phone = "0909000005",
                        Status = 1,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Stations.AddRangeAsync(stations);
                await context.SaveChangesAsync();
            }
        }
    }
}
