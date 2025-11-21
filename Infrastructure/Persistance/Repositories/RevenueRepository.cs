using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories
{
    public class RevenueRepository : IRevenueRepository
    {
        private readonly EVSwappingV2Context _context;

        public RevenueRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(p => p.PaidAt >= startDate && p.PaidAt <= endDate)
                .SumAsync(p => p.Amount);
        }

        public async Task<Dictionary<DateTime, decimal>> GetRevenuePerDayAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(p => p.PaidAt >= startDate && p.PaidAt <= endDate)
                .GroupBy(p => p.PaidAt!.Value.Date)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(p => p.Amount));
        }

        public async Task<Dictionary<DateTime, decimal>> GetRevenuePerMonthAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(p => p.PaidAt >= startDate && p.PaidAt <= endDate)
                .GroupBy(p => new DateTime(p.PaidAt!.Value.Year, p.PaidAt.Value.Month, 1))
                .ToDictionaryAsync(g => g.Key, g => g.Sum(p => p.Amount));
        }
    }
}
