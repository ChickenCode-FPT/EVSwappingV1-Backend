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
    public class OverdueFeeJob : IJob
    {
        private readonly ISwapTransactionRepository _swapRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<OverdueFeeJob> _logger;

        // Có thể config ngoài appsettings
        private const decimal HourlyOverdueRate = 10000m; // 10.000đ / giờ

        public OverdueFeeJob(
            ISwapTransactionRepository swapRepo,
            IPaymentRepository paymentRepo,
            IPaymentService paymentService,
            ILogger<OverdueFeeJob> logger)
        {
            _swapRepo = swapRepo;
            _paymentRepo = paymentRepo;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("=== [OverdueFeeJob] Tick at {time} ===", now);

            var swaps = await _swapRepo.GetAll();

            var completed = swaps.Where(s =>
                s.SwapStatus == SwapStatus.Completed &&
                s.SwapFinishedAt.HasValue).ToList();

            foreach (var swap in completed)
            {
                try
                {
                    var overdueHours = (now - swap.SwapFinishedAt!.Value).TotalHours;
                    if (overdueHours < 1) continue; // chưa đến 1h => không tính phí

                    var payments = await _paymentRepo.GetBySwapTransaction(swap.SwapTransactionId);
                    bool hasPenalty = payments.Any(p => p.Type == PaymentType.Penalty);

                    if (hasPenalty)
                    {
                        _logger.LogInformation($"[Skip] Swap #{swap.SwapTransactionId} already has penalty.");
                        continue;
                    }

                    var hoursRounded = Math.Ceiling(overdueHours);
                    var fee = (decimal)hoursRounded * HourlyOverdueRate;

                    var penaltyDto = new PenaltyPaymentDto
                    {
                        UserId = swap.CustomerUserId,
                        SwapTransactionId = swap.SwapTransactionId,
                        Amount = fee,
                        Reason = $"Overdue battery return ({hoursRounded}h × {HourlyOverdueRate:N0}₫)",
                        Type = PaymentType.Penalty
                    };

                    await _paymentService.CreatePenalty(penaltyDto);

                    _logger.LogInformation($"[Penalty] Swap #{swap.SwapTransactionId} overdue {hoursRounded}h → Fee {fee:N0}₫");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[Error] Calculating overdue fee for Swap #{swap.SwapTransactionId}");
                }
            }
        }
    }
}
