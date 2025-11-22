using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Application.Common.Interfaces.Services;
using Application.Dtos.Payment;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IReservationRepository _reservationRepo;
        private readonly IPaymentGatewayClient _gateway;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<PaymentService> _logger;
        private readonly ISwapTransactionRepository _swapRepo;

        public PaymentService(
            IPaymentRepository paymentRepo,
            IReservationRepository reservationRepo,
            IPaymentGatewayClient gateway,
            IMapper mapper,
            ICurrentUserService currentUser,
            ILogger<PaymentService> logger,
            ISwapTransactionRepository swapRepo)
        {
            _paymentRepo = paymentRepo;
            _reservationRepo = reservationRepo;
            _gateway = gateway;
            _mapper = mapper;
            _currentUser = currentUser;
            _logger = logger;
            _swapRepo = swapRepo;
        }

        public async Task<PaymentResponseDto> CreatePayment(PaymentCreateDto dto)
        {
            var payment = _mapper.Map<Payment>(dto);

            payment.TransactionRef = $"PAY-{Guid.NewGuid():N}".Substring(0, 18).ToUpper();
            payment.Status = PaymentStatus2.Pending;
            payment.CreatedAt = DateTime.UtcNow;

            await _paymentRepo.Add(payment);

            var gatewayResp = await _gateway.CreatePaymentRequestAsync(payment);
            payment.PayOSOrderCode = gatewayResp.GatewayOrderCode;
            payment.CheckoutUrl = gatewayResp.CheckoutUrl;

            await _paymentRepo.Update(payment);
            await _paymentRepo.SaveChanges();

            return gatewayResp;
        }

        public async Task<PaymentResponseDto?> UpdatePaymentStatus(PaymentStatusUpdateDto dto)
        {
            var payment = await _paymentRepo.GetByTransactionRef(dto.OrderCode);
            if (payment == null)
            {
                return null;
            }

            if (payment.Status == PaymentStatus2.Paid)
            {
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

                case "CANCELLED":
                case "FAILED":
                    payment.Status = PaymentStatus2.Cancelled;
                    await _paymentRepo.Update(payment);
                    break;
            }

            await _paymentRepo.SaveChanges();
            scope.Complete();

            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task UpdateLinkedEntitiesAfterPayment(Payment payment)
        {
            if (payment.Status != PaymentStatus2.Paid)
                return;

            if (!payment.ReservationId.HasValue)
                return;

            var res = await _reservationRepo.GetById(payment.ReservationId.Value);
            if (res == null)
                return;

            if (res.Status == ReservationStatus.Pending)
            {
                res.Status = ReservationStatus.Confirmed;
                res.UpdatedAt = DateTime.UtcNow;
                await _reservationRepo.Update(res);
            }

            var swapTx = new SwapTransaction
            {
                ReservationId = res.ReservationId,
                StationId = res.StationId,
                CustomerUserId = res.UserId,
                StaffUserId = null,
                OutgoingBatteryId = null,
                IncomingBatteryId = null,
                SwapStartedAt = DateTime.UtcNow,
                SwapStatus = SwapStatus.Pending,
                Price = payment.Amount,
                Notes = "Swap fee paid with reservation.",
                PaymentType = PaymentType.SwapFee,
                IsPenalty = false,
                CreatedAt = DateTime.UtcNow,
            };

            await _swapRepo.Add(swapTx);
            await _swapRepo.SaveChanges();
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetUserPayments(string userId)
        {
            var payments = await _paymentRepo.GetByUser(userId);
            return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
        }

        public async Task<PaymentResponseDto?> GetPaymentById(long id)
        {
            var payment = await _paymentRepo.GetById(id);
            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetMyPayments()
        {
            var userId = _currentUser.UserId;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("ko xac dinh user.");
            }

            var payments = await _paymentRepo.GetUserPayments(userId);

            return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
        }

        public async Task<IEnumerable<PaymentAndTranDto>> GetPaymentAndSwap()
        {
            var payments = await _paymentRepo.GetFilterWithSwapt();

            return _mapper.Map<IEnumerable<PaymentAndTranDto>>(payments);
        }

        public async Task<PaymentResponseDto?> CompleteOfflinePayment(PaymentManualCompleteDto dto)
        {
            var payment = await _paymentRepo.GetById(dto.PaymentId);
            if (payment == null)
                return null;

            if (payment.Status == PaymentStatus2.Paid)
                return _mapper.Map<PaymentResponseDto>(payment);

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            payment.Status = PaymentStatus2.Paid;
            payment.Method = dto.Method;            
            payment.PaidAt = dto.PaidAt ?? DateTime.UtcNow;
            payment.CreatedAt = DateTime.UtcNow;
            payment.Description = $"Offline payment confirmed by staff {dto.StaffUserId}";

            await _paymentRepo.Update(payment);

            await UpdateLinkedEntitiesAfterPayment(payment);

            await _paymentRepo.SaveChanges();
            scope.Complete();

            return _mapper.Map<PaymentResponseDto>(payment);
        }
    }
}
