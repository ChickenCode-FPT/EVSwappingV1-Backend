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

            var upcomingReservations = await _reservationRepo.GetPendingReservationsBetween(now, now.AddMinutes(HoldAheadMinutes));
            foreach (var res in upcomingReservations)
            {
                try
                {
                    foreach (var alloc in res.ReservationAllocations.Where(a => a.Status == ReservationAllocationStatus.Active))
                    {
                        await _inventoryRepo.MarkHeld(alloc.BatteryId, res.StationId, res.ReservationId);
                        _logger.LogInformation($"[Hold] Battery #{alloc.BatteryId} at Station #{res.StationId} for Reservation #{res.ReservationId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[Error] Holding battery for Reservation #{res.ReservationId}");
                }
            }

            var expiredAllocations = await _allocationRepo.GetExpiredAllocations(now);
            foreach (var alloc in expiredAllocations)
            {
                try
                {
                    alloc.Status = ReservationAllocationStatus.Expired;

                    if (alloc.Reservation != null)
                    {
                        alloc.Reservation.Status = ReservationStatus.Expired;
                        await _inventoryRepo.MarkFull(alloc.BatteryId, alloc.Reservation.StationId);
                        _logger.LogInformation($"[Expire] Released Battery #{alloc.BatteryId} from Station #{alloc.Reservation.StationId}");
                    }

                    await _allocationRepo.Update(alloc);
                    if (alloc.Reservation != null)
                    {
                        await _reservationRepo.Update(alloc.Reservation);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[Error] Expiring Allocation #{alloc.ReservationAllocationId}");
                }
            }

            var pendingReservations = await _reservationRepo.GetPendingReservations();
            foreach (var res in pendingReservations)
            {
                try
                {
                    bool hasSwap = await _swapRepo.ExistsByReservationId(res.ReservationId);
                    if (hasSwap)
                    {
                        res.Status = ReservationStatus.Completed;
                        res.UpdatedAt = now;
                        await _reservationRepo.Update(res);
                        _logger.LogInformation($"[Complete] Reservation #{res.ReservationId} marked as Completed.");
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
