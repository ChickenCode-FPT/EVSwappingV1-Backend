using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Seeder
{
    public class AppSeeder
    {
        public static async Task SeedAllAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var dbContext = services.GetRequiredService<EVSwappingV2Context>();

            // Identity
            await IdentitySeeder.SeedRolesAsync(roleManager);
            await IdentitySeeder.SeedAdminAsync(userManager, roleManager);

            // Stations
            await StationSeeder.SeedAsync(dbContext);

            // Batteries
            await BatterySeeder.SeedAsync(dbContext);
        }
    }
}
