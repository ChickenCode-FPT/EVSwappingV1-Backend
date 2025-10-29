using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Dtos;
using Application.Interfaces.Repositories;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class RefundProcessorJob : IJob
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<RefundProcessorJob> _logger;

        private const int ForfeitThresholdMinutes = 15; // < 15 phút => mất cọc

        public RefundProcessorJob(
            IReservationRepository reservationRepo,
            IPaymentRepository paymentRepo,
            IPaymentService paymentService,
            ILogger<RefundProcessorJob> logger)
        {
            _reservationRepo = reservationRepo;
            _paymentRepo = paymentRepo;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("=== [RefundProcessorJob] Tick at {time} ===", now);

            var cancelledReservations = (await _reservationRepo.GetAll())
                .Where(r => r.Status == ReservationStatus.Cancelled)
                .ToList();

            if (!cancelledReservations.Any())
            {
                _logger.LogInformation("[Refund] No cancelled reservations found.");
                return;
            }

            foreach (var reservation in cancelledReservations)
            {
                try
                {
                    var payments = await _paymentRepo.GetByReservation(reservation.ReservationId);
                    var deposit = payments.FirstOrDefault(p => p.Type == PaymentType.ReservationDeposit);

                    if (deposit == null)
                    {
                        _logger.LogInformation($"[Refund] No deposit for Reservation #{reservation.ReservationId}");
                        continue;
                    }

                    if (deposit.Status is PaymentStatus2.Refunded or PaymentStatus2.Forfeit)
                        continue;

                    var minutesBeforeStart = (reservation.ReservedFrom - now).TotalMinutes;

                    if (minutesBeforeStart < ForfeitThresholdMinutes)
                    {
                        deposit.Status = PaymentStatus2.Forfeit;
                        deposit.Description = "Late cancellation — deposit forfeited";
                        await _paymentRepo.Update(deposit);
                        _logger.LogInformation($"[Forfeit] Reservation #{reservation.ReservationId}: deposit forfeited.");
                        continue;
                    }

                    var refundRequest = new RefundRequestDto
                    {
                        OriginalPaymentId = deposit.PaymentId,
                        RefundAmount = deposit.Amount,
                        Reason = "Reservation cancelled before start time"
                    };

                    var refundResult = await _paymentService.CreateRefund(refundRequest);

                    if (refundResult.Status == PaymentStatus2.Refunded || refundResult.Success)
                    {
                        deposit.Status = PaymentStatus2.Refunded;
                        deposit.Description = "Deposit refunded via VNPAY";
                        await _paymentRepo.Update(deposit);
                        _logger.LogInformation($"[RefundOK] Deposit #{deposit.PaymentId} refunded via VNPAY.");
                    }
                    else
                    {
                        deposit.Status = PaymentStatus2.Cancelled;
                        deposit.Description = $"Refund failed: {refundResult.Description}";
                        await _paymentRepo.Update(deposit);
                        _logger.LogWarning($"[RefundFail] Deposit #{deposit.PaymentId} refund failed: {refundResult.Description}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[RefundError] Reservation #{reservation.ReservationId}");
                }
            }

            await _paymentRepo.SaveChanges();
        }
    }
}
