using Application.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly EVSwappingV2Context _context;

        public PaymentRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task Add(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Payment payment)
        {
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with id={id} not found.");

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Payment>> GetAll()
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.SwapTransaction)
                .Include(p => p.Reservation)
                .Include(p => p.Subscription)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetFilterWithSwapt()
        {
            return await _context.Payments
                .Include(p => p.SwapTransaction)
                .Include(p => p.User)
                .Where(p => p.SwapTransactionId != null)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment?> GetById(long id)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.SwapTransaction)
                .Include(p => p.Reservation)
                .Include(p => p.Subscription)
                .Include(p => p.ParentPayment)
                .Include(p => p.ChildPayments)
                .FirstOrDefaultAsync(p => p.PaymentId == id);
        }

        public async Task<Payment?> GetByTransactionRef(string transactionRef)
        {
            if (string.IsNullOrWhiteSpace(transactionRef))
                return null;

            transactionRef = transactionRef.Trim().ToUpper();

            return await _context.Payments
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.TransactionRef.ToUpper() == transactionRef);
        }

        public async Task<IEnumerable<Payment>> GetByUser(string userId)
        {
            return await _context.Payments
                .Include(p => p.SwapTransaction)
                .Include(p => p.Reservation)
                .Include(p => p.Subscription)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetBySwapTransaction(long swapTransactionId)
        {
            return await _context.Payments
                .Where(p => p.SwapTransactionId == swapTransactionId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByReservation(int reservationId)
        {
            return await _context.Payments
                .Where(p => p.ReservationId == reservationId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetBySubscription(int subscriptionId)
        {
            return await _context.Payments
                .Where(p => p.SubscriptionId == subscriptionId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPendingPayments()
        {
            return await _context.Payments
                .Where(p => p.Status == Domain.Enums.PaymentStatus2.Pending)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetRefundablePayments()
        {
            return await _context.Payments
                .Where(p => p.Type == Domain.Enums.PaymentType.Refund && p.Status == Domain.Enums.PaymentStatus2.Pending)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByDateRange(DateTime start, DateTime end)
        {
            return await _context.Payments
                .Where(p => p.CreatedAt >= start && p.CreatedAt <= end)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
