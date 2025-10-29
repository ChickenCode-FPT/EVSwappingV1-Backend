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
                Console.WriteLine("👉 ReservationSeeder: dữ liệu đã tồn tại, bỏ qua.");
                return;
            }

            Console.WriteLine("🚀 Bắt đầu seed Reservation, Allocation và Payment...");

            var rnd = new Random();

            // lấy 3 user đầu tiên
            var users = await context.Users
                .OrderBy(u => u.CreatedAt)
                .Take(3)
                .ToListAsync();

            if (!users.Any())
                throw new InvalidOperationException("⚠️ Chưa có User nào — hãy chạy UserDriverVehicleSeeder trước.");

            var vehicle = await context.Vehicles.FirstAsync();
            var station = await context.Stations.FirstAsync();

            // lấy ít nhất 3 pin full để seed allocation
            var fullBatteries = await context.Batteries
                .Where(b => b.Status == BatteryStatus.Full)
                .Take(3)
                .ToListAsync();

            if (fullBatteries.Count < 3)
                throw new InvalidOperationException("⚠️ Không đủ pin Full để seed ReservationAllocation.");

            for (int i = 0; i < 3; i++)
            {
                var user = users[i];
                var battery = fullBatteries[i];

                var start = DateTime.UtcNow.AddHours(i + 1);
                var end = start.AddMinutes(45);

                // 1️⃣ Reservation
                var reservation = new Reservation
                {
                    UserId = user.Id,
                    StationId = station.StationId,
                    VehicleId = vehicle.VehicleId,
                    ReservedFrom = start,
                    ReservedTo = end,
                    ReservedBatteryModelId = battery.BatteryModelId,
                    Status = ReservationStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow
                };
                await context.Reservations.AddAsync(reservation);
                await context.SaveChangesAsync();

                // 2️⃣ ReservationAllocation
                var allocation = new ReservationAllocation
                {
                    ReservationId = reservation.ReservationId,
                    BatteryId = battery.BatteryId,
                    AllocatedAt = DateTime.UtcNow,
                    HoldUntil = end,
                    Status = ReservationAllocationStatus.Active
                };
                await context.ReservationAllocations.AddAsync(allocation);
                await context.SaveChangesAsync();

                // 3️⃣ Payment (Deposit)
                var payment = new Payment
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
                    CreatedAt = DateTime.UtcNow,
                    PaidAt = DateTime.UtcNow
                };
                await context.Payments.AddAsync(payment);
                await context.SaveChangesAsync();

                Console.WriteLine($"✅ Seeded Reservation #{reservation.ReservationId} for {user.Email} with Battery #{battery.BatteryId}");
            }

            Console.WriteLine("🎉 ReservationSeeder hoàn tất!");
        }
    }
}
