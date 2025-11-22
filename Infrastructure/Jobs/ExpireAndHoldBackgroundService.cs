using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class ExpireAndHoldBackgroundService : IJob
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IReservationAllocationRepository _allocationRepo;
        private readonly IStationInventoryRepository _inventoryRepo;
        private readonly ISwapTransactionRepository _swapRepo;
        private readonly ILogger<ExpireAndHoldBackgroundService> _logger;

        private const int HoldAheadMinutes = 10;
        private const int HoldDurationMinutes = 15;

        public ExpireAndHoldBackgroundService(
            IReservationRepository reservationRepo,
            IReservationAllocationRepository allocationRepo,
            IStationInventoryRepository inventoryRepo,
            ISwapTransactionRepository swapRepo,
            ILogger<ExpireAndHoldBackgroundService> logger)
        {
            _reservationRepo = reservationRepo;
            _allocationRepo = allocationRepo;
            _inventoryRepo = inventoryRepo;
            _swapRepo = swapRepo;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("=== [ExpireAndHoldJob] Tick at {time} ===", now);

            // hold pin cho reservation sắp đến giờ
            var upcomingReservations = await _reservationRepo.GetPendingReservationsBetween(now, now.AddMinutes(HoldAheadMinutes));

            foreach (var res in upcomingReservations)
            {
                try
                {
                    if (res.Status != ReservationStatus.Pending) continue;

                    foreach (var alloc in res.ReservationAllocations
                                             .Where(a => a.Status == ReservationAllocationStatus.Active))
                    {
                        await _inventoryRepo.MarkHeld(alloc.BatteryId, res.StationId, res.ReservationId);
                    }

                    _logger.LogInformation($"[Hold] Reservation #{res.ReservationId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[Error] Holding battery for Reservation #{res.ReservationId}");
                }
            }

            // expire các allocation quá hạn
            var expiredAllocations = await _allocationRepo.GetExpiredAllocations(now);

            foreach (var alloc in expiredAllocations)
            {
                try
                {
                    alloc.Status = ReservationAllocationStatus.Expired;
                    await _allocationRepo.Update(alloc);

                    if (alloc.Reservation != null)
                    {
                        alloc.Reservation.Status = ReservationStatus.Expired;
                        alloc.Reservation.UpdatedAt = now;
                        await _reservationRepo.Update(alloc.Reservation);

                        await _inventoryRepo.MarkFull(alloc.BatteryId, alloc.Reservation.StationId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[Error] Expiring Allocation #{alloc.ReservationAllocationId}");
                }
            }

            // reservation đã có swap transaction -> Completed
            var pendingReservations = await _reservationRepo.GetPendingReservations();

            foreach (var res in pendingReservations)
            {
                try
                {
                    if (await _swapRepo.ExistsByReservationId(res.ReservationId))
                    {
                        res.Status = ReservationStatus.Completed;
                        res.UpdatedAt = now;
                        await _reservationRepo.Update(res);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[Error] Completing Reservation #{res.ReservationId}");
                }
            }

            await _allocationRepo.SaveChanges();
            await _reservationRepo.SaveChanges();

            _logger.LogInformation("=== [ExpireAndHoldJob] Cycle completed ===");
        }
    }
}
