using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
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
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepo,
            IReservationRepository reservationRepo,
            IPaymentGatewayClient gateway,
            IMapper mapper,
            ILogger<PaymentService> logger)
        {
            _paymentRepo = paymentRepo;
            _reservationRepo = reservationRepo;
            _gateway = gateway;
            _mapper = mapper;
            _logger = logger;
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
            if (payment == null) return null;

            if (payment.Status == PaymentStatus2.Paid)
                return _mapper.Map<PaymentResponseDto>(payment);

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

            if (payment.ReservationId.HasValue)
            {
                var res = await _reservationRepo.GetById(payment.ReservationId.Value);
                if (res != null && res.Status == ReservationStatus.Pending)
                {
                    res.Status = ReservationStatus.Confirmed;
                    res.UpdatedAt = DateTime.UtcNow;
                    await _reservationRepo.Update(res);
                    _logger.LogInformation($"[Payment] Reservation #{res.ReservationId} marked as Confirmed after payment.");
                }
            }

            await _paymentRepo.SaveChanges();
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetUserPayments(string userId)
        {
            var payments = await _paymentRepo.GetByUser(userId);
            return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
        }

        public async Task<PaymentResponseDto?> GetPaymentById(long id)
        {
            var entity = await _paymentRepo.GetById(id);
            return _mapper.Map<PaymentResponseDto>(entity);
        }
    }
}
