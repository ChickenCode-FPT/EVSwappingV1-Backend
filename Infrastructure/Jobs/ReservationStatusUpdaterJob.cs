using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class ReservationStatusUpdaterJob : IJob
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly ISwapTransactionRepository _transactionRepo;
        private readonly ILogger<ReservationStatusUpdaterJob> _logger;

        public ReservationStatusUpdaterJob(
            IReservationRepository reservationRepo,
            ISwapTransactionRepository transactionRepo,
            ILogger<ReservationStatusUpdaterJob> logger)
        {
            _reservationRepo = reservationRepo;
            _transactionRepo = transactionRepo;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("[ReservationStatusUpdaterJob] Started at {time}", DateTime.UtcNow);

            var now = DateTime.UtcNow;
            var pending = await _reservationRepo.GetPendingReservations();

            int updated = 0;

            foreach (var res in pending)
            {
                try
                {
                    bool hasTransaction = await _transactionRepo.ExistsByReservationId(res.ReservationId);

                    if (hasTransaction)
                    {
                        res.Status = ReservationStatus.Completed;
                        updated++;
                    }
                    else if (res.ReservedTo < now)
                    {
                        res.Status = ReservationStatus.Expired;
                        updated++;
                    }

                    res.UpdatedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating reservation {res.ReservationId}");
                }
            }

            if (updated > 0)
                await _reservationRepo.SaveChanges();

            _logger.LogInformation($"[ReservationStatusUpdaterJob] Completed. Updated {updated} reservations.");
        }
    }
}
