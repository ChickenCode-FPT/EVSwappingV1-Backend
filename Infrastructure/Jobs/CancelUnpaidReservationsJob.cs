using Application.Common.Interfaces.Repositories;
using Application.Interfaces.Repositories;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class CancelUnpaidReservationsJob : IJob
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IReservationAllocationRepository _allocationRepo;
        private readonly IStationInventoryRepository _inventoryRepo;
        private readonly ILogger<CancelUnpaidReservationsJob> _logger;

        //private const int UnpaidMinutes = 5;
        private const int UnpaidMinutes = 5;

        public CancelUnpaidReservationsJob(
            IReservationRepository reservationRepo,
            IPaymentRepository paymentRepo,
            IReservationAllocationRepository allocationRepo,
            IStationInventoryRepository inventoryRepo,
            ILogger<CancelUnpaidReservationsJob> logger)
        {
            _reservationRepo = reservationRepo;
            _paymentRepo = paymentRepo;
            _allocationRepo = allocationRepo;
            _inventoryRepo = inventoryRepo;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddMinutes(-UnpaidMinutes);

            _logger.LogInformation("=== [CancelUnpaidReservationsJob] Tick at {time} ===", now);

            var expiredPending = await _reservationRepo.GetUnpaidExpiredReservations(threshold);

            foreach (var res in expiredPending)
            {
                try
                {
                    var payment = await _paymentRepo.GetDepositPaymentForReservation(res.ReservationId);

                    if (payment != null && payment.Status == PaymentStatus2.Pending)
                    {
                        payment.Status = PaymentStatus2.Cancelled;
                        await _paymentRepo.Update(payment);
                    }

                    var allocations = await _allocationRepo.GetByReservationId(res.ReservationId);

                    foreach (var alloc in allocations)
                    {
                        alloc.Status = ReservationAllocationStatus.Expired;

                        await _inventoryRepo.MarkFull(alloc.BatteryId, res.StationId);

                        await _allocationRepo.Update(alloc);
                    }

                    res.Status = ReservationStatus.Cancelled;
                    res.UpdatedAt = now;
                    await _reservationRepo.Update(res);

                    _logger.LogInformation($"[CancelUnpaid] Reservation #{res.ReservationId} cancelled due to unpaid timeout.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[Error] Cancel unpaid Reservation #{res.ReservationId}");
                }
            }

            await _allocationRepo.SaveChanges();
            await _reservationRepo.SaveChanges();
            await _paymentRepo.SaveChanges();

            _logger.LogInformation("=== [CancelUnpaidReservationsJob] Completed ===");
        }
    }
}
