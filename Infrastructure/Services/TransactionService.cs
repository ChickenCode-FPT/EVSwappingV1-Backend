using Application.Common.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TransactionService //: ISwapTransactionService
    {
        private readonly EVSwappingV2Context _db;
        public TransactionService(EVSwappingV2Context db) => _db = db;

        public async Task<SwapTransaction> Create(SwapTransaction tx, CancellationToken ct)
        {
            _db.SwapTransactions.Add(tx);
            await _db.SaveChangesAsync(ct);
            return tx;
        }

        public async Task<SwapTransaction?> GetById(long id, CancellationToken ct) =>
            await _db.SwapTransactions.Include(t => t.Payments).FirstOrDefaultAsync(t => t.SwapTransactionId == id, ct);

        public async Task<List<SwapTransaction>> Query(int? stationId, string? status, DateTime? from, DateTime? to, CancellationToken ct)
        {
            var q = _db.SwapTransactions.AsQueryable();
            if (stationId.HasValue) q = q.Where(t => t.StationId == stationId.Value);
            if (!string.IsNullOrEmpty(status)) q = q.Where(t => t.SwapStatus == status);
            if (from.HasValue) q = q.Where(t => t.SwapStartedAt >= from.Value);
            if (to.HasValue) q = q.Where(t => t.SwapStartedAt <= to.Value);
            return await q.Include(t => t.Payments).ToListAsync(ct);
        }

        public async Task Update(SwapTransaction tx, CancellationToken ct)
        {
            _db.SwapTransactions.Update(tx);
            await _db.SaveChangesAsync(ct);
        }
    }
}
