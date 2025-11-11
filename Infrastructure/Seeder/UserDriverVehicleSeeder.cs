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

            // Staff
            if (!context.Users.Any(u => u.Email!.StartsWith("staff")))
            {
                for (int i = 1; i <= 5; i++)
                {
                    var email = $"staff{i}@gmail.com";
                    if (await userManager.FindByEmailAsync(email) != null) continue;

                    var user = new User
                    {
                        UserName = email,
                        Email = email,
                        FullName = $"Staff {i}",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await userManager.CreateAsync(user, "Staff@123");
                    await userManager.AddToRoleAsync(user, "Staff");
                }
            }

            // Customers
            var existingCustomers = context.Users.Count(u => u.Email!.StartsWith("customer"));
            if (existingCustomers < 10)
            {
                var customers = new List<User>();
                for (int i = existingCustomers + 1; i <= 10; i++)
                {
                    var email = $"customer{i}@gmail.com";
                    var user = new User
                    {
                        UserName = email,
                        Email = email,
                        FullName = $"Customer {i}",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    customers.Add(user);
                    await userManager.CreateAsync(user, "Customer@123");
                    await userManager.AddToRoleAsync(user, "Customer");
                }

                await context.SaveChangesAsync();

                // Drivers & Vehicles
                var batteryModel = await context.BatteryModels.FirstAsync();
                var rnd = new Random();
                int year = DateTime.Now.Year - 1;

                foreach (var customer in customers)
                {
                    await context.Drivers.AddAsync(new Driver
                    {
                        UserId = customer.Id,
                        PreferredPaymentMethod = "VNPAY",
                        TotalSwaps = 0,
                        CreatedAt = DateTime.UtcNow
                    });

                    await context.Vehicles.AddAsync(new Vehicle
                    {
                        UserId = customer.Id,
                        Vin = $"VF{rnd.Next(10000, 99999)}",
                        Make = "VinFast",
                        Model = "VF e34",
                        Year = year,
                        BatteryModelPreferenceId = batteryModel.BatteryModelId,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
