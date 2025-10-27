using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Security;
using System.Transactions;

namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IReservationRepository _reservationRepo;
        private readonly ISwapTransactionRepository _swapRepo;
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly IPaymentGatewayClient _gateway;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepo,
            IReservationRepository reservationRepo,
            ISwapTransactionRepository swapRepo,
            ISubscriptionRepository subscriptionRepo,
            IPaymentGatewayClient gateway,
            IMapper mapper,
            ILogger<PaymentService> logger)
        {
            _paymentRepo = paymentRepo;
            _reservationRepo = reservationRepo;
            _swapRepo = swapRepo;
            _subscriptionRepo = subscriptionRepo;
            _gateway = gateway;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaymentResponseDto> CreatePayment(PaymentCreateDto dto)
        {
            var payment = _mapper.Map<Payment>(dto);

            if (string.IsNullOrEmpty(payment.TransactionRef))
            {
                payment.TransactionRef = $"PAY-{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}";
            }

            payment.Status = PaymentStatus2.Pending;
            payment.CreatedAt = DateTime.UtcNow;

            await _paymentRepo.Add(payment);

            var gatewayResp = await _gateway.CreatePaymentRequestAsync(payment);

            if (!string.IsNullOrEmpty(gatewayResp.GatewayOrderCode))
                payment.PayOSOrderCode = gatewayResp.GatewayOrderCode;

            if (!string.IsNullOrEmpty(gatewayResp.CheckoutUrl))
                payment.CheckoutUrl = gatewayResp.CheckoutUrl;  

            await _paymentRepo.Update(payment);
            await _paymentRepo.SaveChanges();

            _logger.LogInformation("[Payment] Created payment #{id} ({desc}) via VNPAY", payment.PaymentId, payment.Description);

            return gatewayResp;
        }

        public async Task<PaymentResponseDto> CreateRefund(RefundRequestDto dto)
        {
            var original = await _paymentRepo.GetById(dto.OriginalPaymentId);
            if (original == null)
                throw new KeyNotFoundException("Original payment not found.");

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var refund = new Payment
            {
                ParentPaymentId = original.PaymentId,
                UserId = original.UserId,
                Amount = -dto.RefundAmount,
                Currency = original.Currency,
                Method = "VNPAY",
                Type = PaymentType.Refund,
                Status = PaymentStatus2.Pending,
                Description = $"Refund for #{original.PaymentId}: {dto.Reason}",
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepo.Add(refund);

            var result = await _gateway.CreateRefundAsync(original, dto);

            if (result.Success)
            {
                refund.Status = PaymentStatus2.Refunded;
                original.Status = PaymentStatus2.Refunded;
                _logger.LogInformation("[Refund] SUCCESS #{id}, for original #{orig}", refund.PaymentId, original.PaymentId);
            }
            else
            {
                refund.Status = PaymentStatus2.Cancelled;
                refund.Description += $" (Fail: {result.Message})";
                _logger.LogWarning("[Refund] FAIL #{id}: {msg}", refund.PaymentId, result.Message);
            }

            await _paymentRepo.Update(refund);
            await _paymentRepo.Update(original);
            await _paymentRepo.SaveChanges();

            scope.Complete();
            return _mapper.Map<PaymentResponseDto>(refund);
        }

        public async Task<PaymentResponseDto> CreatePenalty(PenaltyPaymentDto dto)
        {
            var penalty = new Payment
            {
                UserId = dto.UserId,
                SwapTransactionId = dto.SwapTransactionId,
                ReservationId = dto.ReservationId,
                Type = PaymentType.Penalty,
                Status = PaymentStatus2.Pending,
                Amount = dto.Amount,
                Currency = "VND",
                Method = "Internal",
                Description = dto.Reason,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepo.Add(penalty);

            penalty.Status = PaymentStatus2.Paid;
            penalty.PaidAt = DateTime.UtcNow;
            await _paymentRepo.Update(penalty);
            await _paymentRepo.SaveChanges();

            await UpdateLinkedEntitiesAfterPayment(penalty);
            return _mapper.Map<PaymentResponseDto>(penalty);
        }

        public async Task SyncPendingPaymentsAsync()
        {
            var pending = await _paymentRepo.GetPendingPayments();
            foreach (var payment in pending)
            {
                if (payment.Status != PaymentStatus2.Pending) continue;

                try
                {
                    var status = await _gateway.GetPaymentStatusAsync(payment.TransactionRef!);
                    if (status == null) continue;

                    switch (status.Status?.ToUpperInvariant())
                    {
                        case "PAID":
                            payment.Status = PaymentStatus2.Paid;
                            payment.PaidAt = DateTime.UtcNow;
                            await UpdateLinkedEntitiesAfterPayment(payment);
                            break;

                        case "FAILED":
                        case "CANCELLED":
                            payment.Status = PaymentStatus2.Cancelled;
                            break;
                    }

                    await _paymentRepo.Update(payment);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Sync] Error while syncing Payment #{id}", payment.PaymentId);
                }
            }

            await _paymentRepo.SaveChanges();
        }

        public async Task UpdateLinkedEntitiesAfterPayment(Payment payment)
        {
            if (payment.Status != PaymentStatus2.Paid &&
                payment.Status != PaymentStatus2.Refunded)
                return;

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (payment.ReservationId.HasValue)
            {
                var res = await _reservationRepo.GetById(payment.ReservationId.Value);
                if (res != null && res.Status == ReservationStatus.Pending)
                {
                    res.Status = ReservationStatus.Completed;
                    await _reservationRepo.Update(res);
                    _logger.LogInformation("[Link] Reservation #{id} marked Completed after payment #{pid}", res.ReservationId, payment.PaymentId);
                }
            }

            if (payment.SwapTransactionId.HasValue)
            {
                var swap = await _swapRepo.GetById(payment.SwapTransactionId.Value);
                if (swap != null && swap.SwapStatus == SwapStatus.Pending)
                {
                    swap.SwapStatus = SwapStatus.Completed;
                    swap.SwapFinishedAt = DateTime.UtcNow;
                    await _swapRepo.Update(swap);
                    _logger.LogInformation("[Link] SwapTransaction #{id} Completed after payment #{pid}", swap.SwapTransactionId, payment.PaymentId);
                }
            }

            if (payment.SubscriptionId.HasValue)
            {
                var sub = await _subscriptionRepo.GetById(payment.SubscriptionId.Value);
                if (sub != null && sub.Status == SubscriptionStatus.Pending)
                {
                    sub.Status = SubscriptionStatus.Active;
                    sub.StartDate = DateTime.UtcNow;
                    sub.EndDate = sub.Package.BillingCycle switch
                    {
                        BillingCycle.Monthly => DateTime.UtcNow.AddMonths(1),
                        BillingCycle.Quarterly => DateTime.UtcNow.AddMonths(3),
                        BillingCycle.Yearly => DateTime.UtcNow.AddYears(1),
                        _ => DateTime.UtcNow.AddMonths(1)
                    };
                    sub.RemainingSwaps = sub.Package.IncludedSwaps;
                    await _subscriptionRepo.Update(sub);
                    _logger.LogInformation("[Link] Subscription #{id} activated via payment #{pid}", sub.SubscriptionId, payment.PaymentId);
                }
            }

            await _paymentRepo.SaveChanges();
            scope.Complete();
        }

        public async Task<IEnumerable<Payment>> GetAllPayments() => await _paymentRepo.GetAll();

        public async Task<PaymentAndTranDto?> GetPaymentById(long id)
        {
            var entity = await _paymentRepo.GetById(id);
            return _mapper.Map<PaymentAndTranDto>(entity);
        }

        public async Task<IEnumerable<PaymentSummaryDto>> GetUserPayments(string userId)
        {
            var payments = await _paymentRepo.GetByUser(userId);
            return _mapper.Map<IEnumerable<PaymentSummaryDto>>(payments);
        }

        public async Task<PaymentResponseDto?> HandleWebhook(PaymentWebhookDto dto)
        {
            //if (!_gateway.VerifyWebhookSignature(dto.RawData, dto.Signature))
            //    throw new SecurityException("Invalid VNPAY signature");

            var payment = await _paymentRepo.GetByTransactionRef(dto.OrderCode);
            if (payment == null) return null;
            if (payment.Status == PaymentStatus2.Paid)
                return _mapper.Map<PaymentResponseDto>(payment);

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (dto.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase))
            {
                payment.Status = PaymentStatus2.Paid;
                payment.PaidAt = DateTime.UtcNow;
                await UpdateLinkedEntitiesAfterPayment(payment);
            }
            else if (dto.Status is "FAILED" or "CANCELLED")
            {
                payment.Status = PaymentStatus2.Cancelled;
            }

            await _paymentRepo.Update(payment);
            await _paymentRepo.SaveChanges();

            scope.Complete();
            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<PaymentResponseDto?> UpdatePaymentStatus(PaymentStatusUpdateDto dto)
        {
            var payment = await _paymentRepo.GetByTransactionRef(dto.OrderCode);
            if (payment == null)
            {
                _logger.LogWarning("[PaymentUpdate] Payment not found for {OrderCode}", dto.OrderCode);
                return null;
            }

            if (payment.Status == PaymentStatus2.Paid && dto.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("[PaymentUpdate] Payment {OrderCode} already Paid, skipping.", dto.OrderCode);
                return _mapper.Map<PaymentResponseDto>(payment);
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            switch (dto.Status.ToUpperInvariant())
            {
                case "PAID":
                case "SUCCESS":
                case "00":
                    payment.Status = PaymentStatus2.Paid;
                    payment.PaidAt = DateTime.UtcNow;
                    await _paymentRepo.Update(payment);
                    await UpdateLinkedEntitiesAfterPayment(payment);
                    break;

                case "REFUNDED":
                    payment.Status = PaymentStatus2.Refunded;
                    payment.PaidAt = DateTime.UtcNow;
                    await _paymentRepo.Update(payment);
                    await UpdateLinkedEntitiesAfterPayment(payment);
                    break;

                case "FORFEIT":
                case "FORFEITED":
                    payment.Status = PaymentStatus2.Forfeit;
                    await _paymentRepo.Update(payment);
                    break;

                case "CANCELLED":
                case "FAILED":
                    payment.Status = PaymentStatus2.Cancelled;
                    await _paymentRepo.Update(payment);
                    break;
            }

            await _paymentRepo.SaveChanges();
            scope.Complete();

            _logger.LogInformation("[PaymentUpdate] Payment #{pid} updated to {status}", payment.PaymentId, payment.Status);
            return _mapper.Map<PaymentResponseDto>(payment);
        }
    }
}
