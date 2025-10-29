using Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Common.Interfaces.Repositories
{
    public interface ISubscriptionRepository
    {
        Task Add2(Subscription subscription);
        Task<Subscription> Add(Subscription subscription);
        Task Update(Subscription subscription);
        Task Delete(int subscriptionId);
        Task SaveChanges();
        Task<Subscription?> GetById(int id);
        Task<IEnumerable<Subscription>> GetByUser(string userId);
        Task<IEnumerable<Subscription>> GetAll();
        Task<Subscription?> GetActiveByUser(string userId);
        Task<IEnumerable<Subscription>> GetExpiringSoon(int daysBefore = 3);
        Task<IEnumerable<Subscription>> GetExpired();
        Task<IEnumerable<Subscription>> GetByDateRange(DateTime start, DateTime end);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
