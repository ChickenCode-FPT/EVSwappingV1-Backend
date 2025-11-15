using Application.Dtos.Payment;
using Domain.Models;

namespace Application.Common.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePayment(PaymentCreateDto dto);
        Task<PaymentResponseDto?> UpdatePaymentStatus(PaymentStatusUpdateDto dto);
        Task UpdateLinkedEntitiesAfterPayment(Payment payment);
        Task<IEnumerable<PaymentResponseDto>> GetUserPayments(string userId);
        Task<PaymentResponseDto?> GetPaymentById(long id);
        Task<IEnumerable<PaymentResponseDto>> GetMyPayments();
    }
}
