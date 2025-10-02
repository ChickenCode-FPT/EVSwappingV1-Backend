using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly EVSwappingV2Context _context;

        public SubscriptionRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<Subscription?> GetActiveByUserIdAsync(string userId)
        {
            return await _context.Subscriptions
                .Include(s => s.Package)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "Active");
        }

        public async Task<Subscription> AddAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task UpdateAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }
    }
}
