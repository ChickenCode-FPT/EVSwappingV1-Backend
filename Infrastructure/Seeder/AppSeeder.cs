using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

            await dbContext.Database.MigrateAsync();

            // Identity
            await IdentitySeeder.SeedRolesAsync(roleManager);
            await IdentitySeeder.SeedAdminAsync(userManager, roleManager);

            // Stations
            await StationSeeder.SeedAsync(dbContext);

            // Batteries
            await BatterySeeder.SeedAsync(dbContext);

            //Seed transaction and payment
            await StatisticSeeder.SeedAsync(dbContext, userManager, roleManager);
        }
    }
}
