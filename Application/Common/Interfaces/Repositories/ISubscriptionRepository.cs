using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<Subscription?> GetActiveByUserIdAsync(string userId);
        Task<Subscription> AddAsync(Subscription subscription);
        Task UpdateAsync(Subscription subscription);
    }
}
