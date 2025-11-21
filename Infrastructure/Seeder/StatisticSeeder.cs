using Bogus;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentStatus = Domain.Enums.PaymentStatus;

namespace Infrastructure.Seeder;

public class StatisticSeeder
{
    private const int TargetUserCount = 1000;
    private const string DefaultPassword = "Customer@123";

    public static async Task SeedAsync(
        EVSwappingV2Context context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        if (await context.SwapTransactions.AnyAsync(s => s.Notes != null && s.Notes.StartsWith("Auto-seeded swap")))
        {
            return; // Seeding has already been done
        }

        await SeedUsersAsync(context, userManager, roleManager);

        var customers = (await userManager.GetUsersInRoleAsync("Customer")).OrderBy(u => u.CreatedAt).ToList();
        var stations = await context.Stations.ToListAsync();
        var batteries = await context.Batteries.Include(b => b.BatteryModel).ToListAsync();

        if (!customers.Any() || !stations.Any() || !batteries.Any())
        {
            // Not enough base data to seed statistics
            return;
        }

        var rnd = new Random();
        var startDate = new DateTime(2025, 6, 1);
        var endDate = DateTime.UtcNow;
        var tempVehicles = new List<Vehicle>();

        for (var day = startDate.Date; day < endDate.Date; day = day.AddDays(1))
        {
            var activeUsers = GetActiveUsersForDay(customers, startDate, day);
            int numSwaps = CalculateSwapsForDay(day, activeUsers.Count, rnd);

            for (int i = 0; i < numSwaps; i++)
            {
                var user = activeUsers[rnd.Next(activeUsers.Count)];
                var vehicle = await GetOrCreateVehicleForUser(context, user, rnd, tempVehicles);
                var swapTime = GetSwapTimeForDay(day, rnd);

                CreateSwapCycle(context, user, vehicle, stations, batteries, swapTime, rnd);
            }
        }

        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedUsersAsync(EVSwappingV2Context context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        const string customerRole = "Customer";
        if (!await roleManager.RoleExistsAsync(customerRole))
        {
            await roleManager.CreateAsync(new IdentityRole(customerRole));
        }

        var existingCustomers = await userManager.GetUsersInRoleAsync(customerRole);
        int usersToCreate = TargetUserCount - existingCustomers.Count;

        if (usersToCreate <= 0)
        {
            return;
        }

        var userFaker = new Faker<User>()
            .RuleFor(u => u.Email, f => f.Internet.Email(f.Name.FirstName(), f.Name.LastName(), Guid.NewGuid().ToString("N").Substring(0, 6)))
            .RuleFor(u => u.UserName, (f, u) => u.Email)
            .RuleFor(u => u.FullName, f => f.Name.FullName())
            .RuleFor(u => u.EmailConfirmed, true)
            .RuleFor(u => u.CreatedAt, f => f.Date.Past(2, new DateTime(2025, 1, 1)));

        for (int i = 0; i < usersToCreate; i++)
        {
            var user = userFaker.Generate();
            var result = await userManager.CreateAsync(user, DefaultPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, customerRole);
            }
        }
    }

    private static List<User> GetActiveUsersForDay(List<User> allCustomers, DateTime startDate, DateTime currentDate)
    {
        var totalDays = (DateTime.UtcNow.Date - startDate.Date).Days;
        if (totalDays <= 0) return allCustomers.GetRange(0, Math.Min(allCustomers.Count, 10));

        var elapsedDays = (currentDate.Date - startDate.Date).Days;
        double growth = Math.Clamp((double)elapsedDays / totalDays, 0.0, 1.0);

        // Start with a base of 10 users and grow
        int baseUsers = 10;
        int availableUserCount = baseUsers + (int)((allCustomers.Count - baseUsers) * growth);
        availableUserCount = Math.Clamp(availableUserCount, 1, allCustomers.Count);

        return allCustomers.GetRange(0, availableUserCount);
    }


    private static int CalculateSwapsForDay(DateTime day, int activeUsers, Random rnd)
    {
        // Average swaps per active user per day
        const double averageSwapsPerUser = 0.04;

        int baseSwaps = (int)(activeUsers * averageSwapsPerUser);
        int amplitude = (int)(baseSwaps * 0.75); // Fluctuation

        // Yearly trend: Cosine wave peaking in mid-February (day ~45)
        double dayOfYear = day.DayOfYear;
        double peakDay = 45;
        double totalDaysInYear = DateTime.IsLeapYear(day.Year) ? 366 : 365;
        double yearlyFactor = Math.Cos(2 * Math.PI * (dayOfYear - peakDay) / totalDaysInYear);

        int calculatedSwaps = (int)(baseSwaps + amplitude * yearlyFactor);

        // Add daily random noise
        calculatedSwaps += rnd.Next(-3, 4);

        return Math.Max(0, calculatedSwaps);
    }

    private static DateTime GetSwapTimeForDay(DateTime day, Random rnd)
    {
        // Weights for each hour of the day to simulate morning and afternoon peaks
        var hourWeights = new[] { 1, 1, 1, 1, 2, 3, 5, 8, 7, 5, 4, 4, 4, 5, 6, 7, 9, 8, 6, 4, 3, 2, 2, 1 };
        int totalWeight = hourWeights.Sum();

        int randomWeight = rnd.Next(totalWeight);
        int hour = 0;
        int cumulativeWeight = 0;
        for (int i = 0; i < hourWeights.Length; i++)
        {
            cumulativeWeight += hourWeights[i];
            if (randomWeight < cumulativeWeight)
            {
                hour = i;
                break;
            }
        }

        var minute = rnd.Next(60);
        var second = rnd.Next(60);
        return day.AddHours(hour).AddMinutes(minute).AddSeconds(second);
    }

    private static async Task<Vehicle> GetOrCreateVehicleForUser(EVSwappingV2Context context, User user, Random rnd, List<Vehicle> tempVehicles)
    {
        var vehicle = tempVehicles.FirstOrDefault(v => v.UserId == user.Id);
        if (vehicle == null)
        {
            vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.UserId == user.Id);
            if (vehicle == null)
            {
                var faker = new Faker();
                vehicle = new Vehicle
                {
                    UserId = user.Id,
                    Vin = faker.Vehicle.Vin().Substring(0, 8),
                    Model = faker.Vehicle.Model(),
                    CreatedAt = user.CreatedAt
                };
                context.Vehicles.Add(vehicle);
                tempVehicles.Add(vehicle);
            }
        }
        return vehicle;
    }

    private static void CreateSwapCycle(
        EVSwappingV2Context context,
        User user,
        Vehicle vehicle,
        List<Station> stations,
        List<Battery> batteries,
        DateTime swapTime,
        Random rnd)
    {
        var station = stations[rnd.Next(stations.Count)];

        var outBattery = batteries[rnd.Next(batteries.Count)];
        var inBattery = batteries[rnd.Next(batteries.Count)];
        while (inBattery.BatteryId == outBattery.BatteryId)
        {
            inBattery = batteries[rnd.Next(batteries.Count)];
        }

        var reservationTime = swapTime.AddMinutes(-rnd.Next(20, 90));

        var reservation = new Reservation
        {
            User = user,
            Station = station,
            Vehicle = vehicle,
            ReservedFrom = reservationTime,
            ReservedTo = reservationTime.AddMinutes(30),
            ReservedBatteryModelId = outBattery.BatteryModel.BatteryModelId,
            Status = ReservationStatus.Completed,
            CreatedAt = reservationTime,
            UpdatedAt = swapTime
        };

        var depositPayment = new Payment
        {
            User = user,
            Reservation = reservation,
            Type = PaymentType.ReservationDeposit,
            Amount = outBattery.BatteryModel.ReservationDepositFee,
            Currency = "VND",
            Method = "VNPAY",
            Status = PaymentStatus.Paid.ToString(),
            TransactionRef = $"SEED-PAY-RES-{Guid.NewGuid()}",
            Description = "Auto-seeded deposit",
            CreatedAt = reservationTime,
            PaidAt = reservationTime
        };

        var swap = new SwapTransaction
        {
            Reservation = reservation,
            Station = station,
            CustomerUser = user,
            OutgoingBattery = outBattery,
            IncomingBattery = inBattery,
            SwapStartedAt = swapTime.AddMinutes(-rnd.Next(2, 5)),
            SwapFinishedAt = swapTime,
            SwapStatus = SwapStatus.Completed,
            Price = rnd.Next(40000, 100000),
            Notes = $"Auto-seeded swap for {user.FullName}",
            CreatedAt = swapTime
        };

        var swapPayment = new Payment
        {
            User = user,
            SwapTransaction = swap,
            Type = PaymentType.SwapFee,
            Amount = swap.Price,
            Currency = "VND",
            Method = "VNPAY",
            Status = PaymentStatus.Paid.ToString(),
            TransactionRef = $"SEED-PAY-SWAP-{Guid.NewGuid()}",
            Description = "Auto-seeded swap fee",
            CreatedAt = swapTime,
            PaidAt = swapTime
        };

        context.Reservations.Add(reservation);
        context.Payments.Add(depositPayment);
        context.SwapTransactions.Add(swap);
        context.Payments.Add(swapPayment);
    }
}