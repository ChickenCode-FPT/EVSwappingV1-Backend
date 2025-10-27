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
                var packages = new List<SubscriptionPackage>
                {
                    new SubscriptionPackage
                    {
                        Name = "Eco Plan",
                        BillingCycle = BillingCycle.Monthly,
                        Price = 150000,
                        IncludedSwaps = 5,
                        CreatedAt = DateTime.UtcNow
                    },
                    new SubscriptionPackage
                    {
                        Name = "Pro Plan",
                        BillingCycle = BillingCycle.Monthly,
                        Price = 250000,
                        IncludedSwaps = 10,
                        CreatedAt = DateTime.UtcNow
                    },
                    new SubscriptionPackage
                    {
                        Name = "Elite Plan",
                        BillingCycle = BillingCycle.Quarterly,
                        Price = 600000,
                        IncludedSwaps = 35,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.SubscriptionPackages.AddRangeAsync(packages);
                await context.SaveChangesAsync();
            }

            // Lấy 2 customer đầu tiên để seed Subscription
            var users = context.Users.Take(2).ToList();
            var ecoPlan = context.SubscriptionPackages.First(p => p.Name == "Eco Plan");
            var proPlan = context.SubscriptionPackages.First(p => p.Name == "Pro Plan");

            if (!context.Subscriptions.Any())
            {
                var subs = new List<Subscription>
                {
                    new Subscription
                    {
                        UserId = users[0].Id,
                        PackageId = ecoPlan.PackageId,
                        Status = SubscriptionStatus.Active,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(1),
                        RemainingSwaps = ecoPlan.IncludedSwaps,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Subscription
                    {
                        UserId = users[1].Id,
                        PackageId = proPlan.PackageId,
                        Status = SubscriptionStatus.Active,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(1),
                        RemainingSwaps = proPlan.IncludedSwaps,
                        CreatedAt = DateTime.UtcNow
                    }
                };
                await context.Subscriptions.AddRangeAsync(subs);
                await context.SaveChangesAsync();
            }
        }
    }
}
