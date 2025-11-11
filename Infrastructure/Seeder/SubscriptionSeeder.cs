using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder
{
    public class SubscriptionSeeder
    {
        public static async Task SeedAsync(EVSwappingV2Context context)
        {
            await context.Database.MigrateAsync();

            if (!context.SubscriptionPackages.Any())
            {
                await context.SubscriptionPackages.AddRangeAsync(new List<SubscriptionPackage>
                {
                    new SubscriptionPackage { Name = "Eco Plan", BillingCycle = BillingCycle.Monthly, Price = 150000, IncludedSwaps = 5, CreatedAt = DateTime.UtcNow },
                    new SubscriptionPackage { Name = "Pro Plan", BillingCycle = BillingCycle.Monthly, Price = 250000, IncludedSwaps = 10, CreatedAt = DateTime.UtcNow },
                    new SubscriptionPackage { Name = "Elite Plan", BillingCycle = BillingCycle.Quarterly, Price = 600000, IncludedSwaps = 35, CreatedAt = DateTime.UtcNow }
                });
                await context.SaveChangesAsync();
            }

            if (!context.Subscriptions.Any())
            {
                var users = await context.Users.Where(u => u.Email!.StartsWith("customer")).Take(3).ToListAsync();
                var eco = await context.SubscriptionPackages.FirstAsync(p => p.Name == "Eco Plan");
                var pro = await context.SubscriptionPackages.FirstAsync(p => p.Name == "Pro Plan");
                var elite = await context.SubscriptionPackages.FirstAsync(p => p.Name == "Elite Plan");

                await context.Subscriptions.AddRangeAsync(new List<Subscription>
                {
                    new Subscription { UserId = users[0].Id, PackageId = eco.PackageId, Status = SubscriptionStatus.Active, StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddMonths(1), RemainingSwaps = eco.IncludedSwaps, CreatedAt = DateTime.UtcNow },
                    new Subscription { UserId = users[1].Id, PackageId = pro.PackageId, Status = SubscriptionStatus.Pending, StartDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
                    new Subscription { UserId = users[2].Id, PackageId = elite.PackageId, Status = SubscriptionStatus.Expired, StartDate = DateTime.UtcNow.AddMonths(-4), EndDate = DateTime.UtcNow.AddMonths(-1), RemainingSwaps = 0, CreatedAt = DateTime.UtcNow }
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
