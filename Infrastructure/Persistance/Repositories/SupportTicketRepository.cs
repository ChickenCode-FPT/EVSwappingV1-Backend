using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Persistance.Repositories;
public class SupportTicketRepository : ISupportTicketRepository
{
    private readonly EVSwappingV2Context _context;
    private readonly DbSet<SupportTicket> _ticketSet;

    public SupportTicketRepository(EVSwappingV2Context context)
    {
        _context = context;
        _ticketSet = _context.SupportTickets;
    }

    public async Task<SupportTicket> AddAsync(SupportTicket ticket)
    {
        await _ticketSet.AddAsync(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<bool> DeleteAsync(long ticketId)
    {
        var ticket = await _ticketSet.FindAsync(ticketId);
        if (ticket == null)
        {
            return false;
        }

        _ticketSet.Remove(ticket);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SupportTicket>> GetAllAsync(Expression<Func<SupportTicket, bool>>? predicate = null, Func<IQueryable<SupportTicket>, IIncludableQueryable<SupportTicket, object>>? include = null)
    {
        IQueryable<SupportTicket> query = _ticketSet;

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync();
    }

    public async Task<SupportTicket?> GetAsync(Expression<Func<SupportTicket, bool>> predicate, Func<IQueryable<SupportTicket>, IIncludableQueryable<SupportTicket, object>>? include = null)
    {
        IQueryable<SupportTicket> query = _ticketSet;

        if (include != null)
        {
            query = include(query);
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<SupportTicket> UpdateAsync(SupportTicket ticket)
    {
        _ticketSet.Update(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }
}
