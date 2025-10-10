using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class ExpiredReservationReleaseJob : IJob
    {
        private readonly IReservationAllocationRepository _allocationRepo;
        private readonly IStationInventoryRepository _inventoryRepo;
        private readonly ILogger<ExpiredReservationReleaseJob> _logger;

        public ExpiredReservationReleaseJob(
            IReservationAllocationRepository allocationRepo,
            IStationInventoryRepository inventoryRepo,
            ILogger<ExpiredReservationReleaseJob> logger)
        {
            _allocationRepo = allocationRepo;
            _inventoryRepo = inventoryRepo;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("[ExpiredReservationReleaseJob] Checking expired allocations at {time}", DateTime.UtcNow);

            var now = DateTime.UtcNow;
            var expired = await _allocationRepo.GetExpiredAllocations(now);

            if (expired == null || !expired.Any())
            {
                _logger.LogInformation("No expired allocations found.");
                return;
            }

            foreach (var alloc in expired)
            {
                try
                {
                    alloc.Status = ReservationAllocationStatus.Expired;

                    if (alloc.Reservation != null)
                        await _inventoryRepo.MarkFull(alloc.BatteryId, alloc.Reservation.StationId);

                    _logger.LogInformation($"Released Battery #{alloc.BatteryId} at Station #{alloc.Reservation?.StationId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error releasing allocation #{alloc.ReservationAllocationId}");
                }
            }

            await _allocationRepo.SaveChanges();
        }
    }
}
