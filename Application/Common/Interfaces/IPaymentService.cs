using Application.Dtos;
using Domain.Models;

namespace Application.Common.Interfaces
{
    public interface IPaymentService
    {
        Task AddPayment(Payment payment);
        Task<PaymentAndTranDto?> GetPaymentById(int id);
        Task<List<Payment>> GetAllPayments();
        Task UpdatePayment(int id, PaymentUpdateDto dto);
        Task DeletePayment(int id);
        Task<IEnumerable<PaymentAndTranDto>> GetFilterWithSwapt();

        Task HandleReturnTransactionAsync(int swapTransactionId, int batteryId, string returnCondition);
    }
}
