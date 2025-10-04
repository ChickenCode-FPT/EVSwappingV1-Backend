using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<Subscription?> GetActiveByUserId(string userId);
        Task<Subscription> Add(Subscription subscription);
        Task Update(Subscription subscription);
    }
}
