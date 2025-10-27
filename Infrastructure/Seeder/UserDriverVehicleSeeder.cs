using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder
{
    public class UserDriverVehicleSeeder
    {
        public static async Task SeedAsync(UserManager<User> userManager, EVSwappingV2Context context)
        {
            await context.Database.MigrateAsync();

            // Nếu chưa có user Customer nào thì seed
            if (!context.Users.Any(u => u.Email!.EndsWith("@gmail.com")))
            {
                var customers = new List<User>();
                for (int i = 1; i <= 5; i++)
                {
                    var email = $"customer{i}@gmail.com";
                    var user = new User
                    {
                        UserName = email,
                        Email = email,
                        FullName = $"Customer {i}",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                    };
                    customers.Add(user);
                    await userManager.CreateAsync(user, "Customer@123");
                    await userManager.AddToRoleAsync(user, "Customer");
                }

                await context.SaveChangesAsync();

                // Drivers
                foreach (var customer in customers)
                {
                    var driver = new Driver
                    {
                        UserId = customer.Id,
                        PreferredPaymentMethod = "VNPAY",
                        TotalSwaps = 0,
                        CreatedAt = DateTime.UtcNow
                    };
                    await context.Drivers.AddAsync(driver);
                }

                await context.SaveChangesAsync();

                // Vehicles
                var batteryModel = await context.BatteryModels.FirstAsync();
                var rnd = new Random();
                int year = DateTime.Now.Year - 1;

                foreach (var customer in customers)
                {
                    var vehicle = new Vehicle
                    {
                        UserId = customer.Id,
                        Vin = $"VIN{rnd.Next(10000, 99999)}",
                        Make = "VinFast",
                        Model = "VF e34",
                        Year = year,
                        BatteryModelPreferenceId = batteryModel.BatteryModelId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await context.Vehicles.AddAsync(vehicle);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
