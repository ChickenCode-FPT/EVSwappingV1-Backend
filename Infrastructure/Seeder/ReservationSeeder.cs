using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder
{
    public class ReservationSeeder
    {
        public static async Task SeedAsync(EVSwappingV2Context context)
        {
            await context.Database.MigrateAsync();

            if (context.Reservations.Any())
            {
                return;
            }

            var rnd = new Random();
            var users = await context.Users
                .Where(u => u.Email!.StartsWith("customer"))
                .ToListAsync();

            var vehicles = await context.Vehicles.ToListAsync();
            var stations = await context.Stations.ToListAsync();

            var fullBatteries = await context.Batteries
                .Where(b => b.Status == BatteryStatus.Full)
                .Take(20)
                .ToListAsync();

            for (int i = 0; i < 8; i++)
            {
                if (!fullBatteries.Any())
                {
                    break;
                }

                var user = users[rnd.Next(users.Count)];
                var vehicle = vehicles.First(v => v.UserId == user.Id);
                var station = stations[rnd.Next(stations.Count)];

                var battery = fullBatteries[rnd.Next(fullBatteries.Count)];
                fullBatteries.Remove(battery);

                var start = DateTime.Now.AddHours(i + 1); 
                var end = start.AddMinutes(30 + rnd.Next(15, 45)); 

                // Tạo Reservation
                var reservation = new Reservation
                {
                    UserId = user.Id,
                    StationId = station.StationId,
                    VehicleId = vehicle.VehicleId,
                    ReservedFrom = start,  
                    ReservedTo = end,     
                    ReservedBatteryModelId = battery.BatteryModelId,
                    Status = ReservationStatus.Confirmed,
                    CreatedAt = DateTime.Now
                };

                await context.Reservations.AddAsync(reservation);
                await context.SaveChangesAsync();

                await context.ReservationAllocations.AddAsync(new ReservationAllocation
                {
                    ReservationId = reservation.ReservationId,
                    BatteryId = battery.BatteryId,
                    AllocatedAt = DateTime.Now,
                    HoldUntil = end, 
                    Status = ReservationAllocationStatus.Active
                });

                await context.Payments.AddAsync(new Payment
                {
                    UserId = user.Id,
                    ReservationId = reservation.ReservationId,
                    Type = PaymentType.ReservationDeposit,
                    Amount = 50000,
                    Currency = "VND",
                    Method = "VNPAY",
                    Status = PaymentStatus2.Paid,
                    TransactionRef = $"PAY-RES-{reservation.ReservationId:D4}",
                    Description = $"Deposit for reservation #{reservation.ReservationId}",
                    CreatedAt = DateTime.Now,
                    PaidAt = DateTime.Now
                });

                await context.SaveChangesAsync();

                var swap = new SwapTransaction
                {
                    ReservationId = reservation.ReservationId,
                    StationId = station.StationId,
                    CustomerUserId = user.Id,
                    OutgoingBatteryId = battery.BatteryId,
                    IncomingBatteryId = battery.BatteryId,
                    SwapStartedAt = DateTime.Now.AddHours(-rnd.Next(1, 4)),   
                    SwapFinishedAt = DateTime.Now.AddHours(-rnd.Next(0, 2)), 
                    SwapStatus = SwapStatus.Completed,
                    Price = rnd.Next(40000, 100000),
                    Notes = $"Auto swap for {user.FullName}",
                    CreatedAt = DateTime.Now
                };

                await context.SwapTransactions.AddAsync(swap);
                await context.SaveChangesAsync();

                await context.Payments.AddAsync(new Payment
                {
                    UserId = user.Id,
                    SwapTransactionId = swap.SwapTransactionId,
                    Type = PaymentType.SwapFee,
                    Amount = swap.Price,
                    Currency = "VND",
                    Method = "VNPAY",
                    Status = PaymentStatus2.Paid,
                    TransactionRef = $"PAY-SWAP-{swap.SwapTransactionId:D4}",
                    Description = $"Swap fee for transaction #{swap.SwapTransactionId}",
                    CreatedAt = DateTime.Now,
                    PaidAt = DateTime.Now
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
