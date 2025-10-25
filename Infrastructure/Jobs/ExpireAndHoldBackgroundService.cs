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
            _logger.LogInformation("=== [ExpireAndHoldBackgroundService] Tick at {time} ===", now);

            var soonReservations = (await _reservationRepo.GetPendingReservations())
                .Where(r => r.ReservedFrom > now && r.ReservedFrom <= now.AddMinutes(10))
                .ToList();

            foreach (var res in soonReservations)
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
                    _logger.LogError(ex, $"Error holding battery for reservation {res.ReservationId}");
                }
            }

            var expiredAllocs = await _allocationRepo.GetExpiredAllocations(now);

            foreach (var alloc in expiredAllocs)
            {
                try
                {
                    alloc.Status = ReservationAllocationStatus.Expired;

                    if (alloc.Reservation != null)
                    {
                        alloc.Reservation.Status = ReservationStatus.Expired;
                        await _inventoryRepo.MarkFull(alloc.BatteryId, alloc.Reservation.StationId);
                        _logger.LogInformation($"[Expire] Released Battery #{alloc.BatteryId} at Station #{alloc.Reservation.StationId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error expiring allocation #{alloc.ReservationAllocationId}");
                }
            }

            var pending = await _reservationRepo.GetPendingReservations();
            foreach (var res in pending)
            {
                try
                {
                    bool swapped = await _swapRepo.ExistsByReservationId(res.ReservationId);
                    if (swapped)
                    {
                        res.Status = ReservationStatus.Completed;
                        res.UpdatedAt = now;
                        _logger.LogInformation($"[Complete] Reservation #{res.ReservationId} marked as Completed");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error completing reservation {res.ReservationId}");
                }
            }

            await _allocationRepo.SaveChanges();
            await _reservationRepo.SaveChanges();
        }
    }
}
