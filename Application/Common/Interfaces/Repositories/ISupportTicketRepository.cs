using Domain.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories
{
    public interface ISupportTicketRepository
    {
        Task<SupportTicket> AddAsync(SupportTicket ticket);
        Task<SupportTicket?> GetAsync(
            Expression<Func<SupportTicket, bool>> predicate,
            Func<IQueryable<SupportTicket>, IIncludableQueryable<SupportTicket, object>>? include = null
        );
        Task<IEnumerable<SupportTicket>> GetAllAsync(
            Expression<Func<SupportTicket, bool>>? predicate = null,
            Func<IQueryable<SupportTicket>, IIncludableQueryable<SupportTicket, object>>? include = null
        );
        Task<SupportTicket> UpdateAsync(SupportTicket ticket);
        Task<bool> DeleteAsync(long ticketId);
    }
}
