using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistance.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly EVSwappingV2Context _context;

        public SubscriptionRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task Add2(Subscription subscription)
        {
            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task<Subscription> Add(Subscription subscription)
        {
            var entry = await _context.Subscriptions.AddAsync(subscription);

            await _context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task Update(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int subscriptionId)
        {
            var sub = await _context.Subscriptions.FindAsync(subscriptionId);
            if (sub == null)
                throw new KeyNotFoundException($"Subscription with id={subscriptionId} not found.");

            _context.Subscriptions.Remove(sub);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChanges() => await _context.SaveChangesAsync();

        public async Task<Subscription?> GetById(int id)
        {
            return await _context.Subscriptions
                .Include(s => s.Package)
                .Include(s => s.Payments)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SubscriptionId == id);
        }

        public async Task<IEnumerable<Subscription>> GetByUser(string userId)
        {
            return await _context.Subscriptions
                .Include(s => s.Package)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetAll()
        {
            return await _context.Subscriptions
                .Include(s => s.Package)
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<Subscription?> GetActiveByUser(string userId)
        {
            return await _context.Subscriptions
                .Include(s => s.Package)
                .FirstOrDefaultAsync(s =>
                    s.UserId == userId &&
                    s.Status == SubscriptionStatus.Active &&
                    (s.EndDate == null || s.EndDate > DateTime.UtcNow));
        }

        public async Task<IEnumerable<Subscription>> GetExpiringSoon(int daysBefore = 3)
        {
            var targetDate = DateTime.UtcNow.AddDays(daysBefore);
            return await _context.Subscriptions
                .Include(s => s.Package)
                .Where(s =>
                    s.Status == SubscriptionStatus.Active &&
                    s.EndDate != null &&
                    s.EndDate.Value.Date <= targetDate.Date)
                .OrderBy(s => s.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetExpired()
        {
            return await _context.Subscriptions
                .Include(s => s.Package)
                .Where(s =>
                    s.Status != SubscriptionStatus.Expired &&
                    s.EndDate != null &&
                    s.EndDate < DateTime.UtcNow)
                .OrderBy(s => s.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetByDateRange(DateTime start, DateTime end)
        {
            return await _context.Subscriptions
                .Include(s => s.Package)
                .Include(s => s.User)
                .Where(s => s.CreatedAt >= start && s.CreatedAt <= end)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        }
    }
}
