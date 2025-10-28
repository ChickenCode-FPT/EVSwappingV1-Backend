using Application.Dtos.Payment;
using Domain.Models;

namespace Application.Common.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePayment(PaymentCreateDto dto);
        Task<PaymentResponseDto> CreateRefund(RefundRequestDto dto);
        Task<PaymentResponseDto> CreatePenalty(PenaltyPaymentDto dto);
        Task SyncPendingPaymentsAsync();
        Task UpdateLinkedEntitiesAfterPayment(Payment payment);
        Task<IEnumerable<Payment>> GetAllPayments();
        Task<PaymentAndTranDto?> GetPaymentById(long id);
        Task<IEnumerable<PaymentSummaryDto>> GetUserPayments(string userId);
        Task<PaymentResponseDto?> HandleWebhook(PaymentWebhookDto dto);
        Task<PaymentResponseDto?> UpdatePaymentStatus(PaymentStatusUpdateDto dto);
    }
}
