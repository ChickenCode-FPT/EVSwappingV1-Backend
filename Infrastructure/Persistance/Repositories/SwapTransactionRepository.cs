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
    public class SwapTransactionRepository : ISwapTransactionRepository
    {
        private readonly EVSwappingV2Context _context;

        public SwapTransactionRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<int> GetSwapCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.SwapTransactions
                .CountAsync(s => s.SwapStartedAt >= startDate && s.SwapStartedAt <= endDate);
        }

        public async Task<Dictionary<int, int>> GetPeakHoursAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.SwapTransactions
                .Where(s => s.SwapStartedAt >= startDate && s.SwapStartedAt <= endDate)
                .GroupBy(s => s.SwapStartedAt.Hour)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }
    }
}
