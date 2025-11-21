using Bogus;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Seeder;

public class StatisticSeeder
{
    public static async Task SeedAsync(
        EVSwappingV2Context context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager
        )
    {
        if (await context.Payments.AnyAsync()
        || await context.SwapTransactions.AnyAsync())
        {
            return;
        }

        string userPass = "Customer@123";
        var userFaker = new Faker<User>()
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.FullName, f => f.Name.FirstName())
            .RuleFor(u => u.EmailConfirmed, f => true)
            .RuleFor(u => u.CreatedAt, f => DateTime.UtcNow);

        var users = userFaker.Generate(50);
        foreach (var user in users)
        {
            var result = await userManager.CreateAsync(user, userPass);
            if (result.Succeeded)
            {
                if (!await roleManager.RoleExistsAsync("Customer"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Customer"));
                }
                await userManager.AddToRoleAsync(user, "Customer");
            }
        }

        var driverIdQueue = new Queue<string>([.. users.Select(d => d.Id)]);



        var userIds = await context.Users.Select(u => u.Id).ToListAsync();
        // var driverIds = await context.Drivers.Select(d => d.DriverId).ToListAsync();
        var drivers = await userManager.GetUsersInRoleAsync("Customer");
        var driverIds = drivers.Select(d => d.Id).ToList();
        var stationIds = await context.Stations.Select(s => s.StationId).ToListAsync();
        var batteryIds = await context.Batteries.Select(b => b.BatteryId).ToListAsync();
        var subscriptionIds = await context.Subscriptions.Select(s => s.SubscriptionId).ToListAsync();

        if (!driverIds.Any() || !stationIds.Any() || !batteryIds.Any())
        {
            // Not enough data to seed statistics
            return;
        }

        var swapTransactionFaker = new Faker<SwapTransaction>()
            .RuleFor(s => s.StationId, f => f.PickRandom(stationIds))
            .RuleFor(s => s.CustomerUserId, f => f.PickRandom(driverIds).ToString())
            .RuleFor(s => s.OutgoingBatteryId, f => f.PickRandom(batteryIds))
            .RuleFor(s => s.IncomingBatteryId, (f, s) =>
            {
                var newBatteryId = f.PickRandom(batteryIds);
                while (newBatteryId == s.OutgoingBatteryId)
                {
                    newBatteryId = f.PickRandom(batteryIds);
                }
                return newBatteryId;
            })
            .RuleFor(s => s.SwapStartedAt, f => f.Date.Between(DateTime.UtcNow.AddMonths(-6), DateTime.UtcNow))
            .RuleFor(s => s.CreatedAt, f => DateTime.UtcNow)
            .RuleFor(s => s.Price, f => f.Finance.Amount(5, 20));

        var swapTransactions = swapTransactionFaker.Generate(1000);
        await context.SwapTransactions.AddRangeAsync(swapTransactions);


        //Seed payments and swap transactions statistics data
        //    Faker p = new();
        var paymentFaker = new Faker<Payment>()
            .RuleFor(p => p.Amount, f => f.Finance.Amount(50, 500))
            .RuleFor(p => p.PaidAt, f => f.Date.Between(DateTime.UtcNow.AddMonths(-6), DateTime.UtcNow))
            .RuleFor(p => p.CreatedAt, (f, p) => p.PaidAt.HasValue ? p.PaidAt.Value.AddMinutes(-5) : DateTime.UtcNow)
            .RuleFor(p => p.UserId, f => userIds.Any() ? f.PickRandom(userIds) : null)
            // .RuleFor(p => p.SubscriptionId, f => f.Random.Bool(0.7f) && subscriptionIds.Any() ? f.PickRandom(subscriptionIds) : (int?)null)
            .RuleFor(p => p.Method, f => f.PickRandom(new[] { "Credit Card", "PayPal", "VNPay" }))
            .RuleFor(p => p.TransactionRef, f => f.Random.Guid().ToString())
            .RuleFor(p => p.Status, f => f.PickRandom(new[] { "Completed", "Pending", "Failed" }));

        var payments = paymentFaker.Generate(500);
        await context.Payments.AddRangeAsync(payments);


        await context.SaveChangesAsync();
    }
}
