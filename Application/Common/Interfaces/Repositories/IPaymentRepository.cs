using Domain.Models;

namespace Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task Add(Payment payment);
        Task Update(Payment payment);
        Task Delete(Payment payment);
        Task Delete(long id);
        Task SaveChanges();
        Task<List<Payment>> GetAll();
        Task<Payment?> GetById(long id);
        Task<Payment?> GetByTransactionRef(string transactionRef);
        Task<IEnumerable<Payment>> GetByUser(string userId);
        Task<IEnumerable<Payment>> GetBySwapTransaction(long swapTransactionId);
        Task<IEnumerable<Payment>> GetByReservation(int reservationId);
        Task<IEnumerable<Payment>> GetBySubscription(int subscriptionId);
        Task<IEnumerable<Payment>> GetFilterWithSwapt();
        Task<IEnumerable<Payment>> GetPendingPayments();
        Task<IEnumerable<Payment>> GetRefundablePayments();
        Task<IEnumerable<Payment>> GetByDateRange(DateTime start, DateTime end);
    }
}
