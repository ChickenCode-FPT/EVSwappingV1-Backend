using Application.Common.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories
{
    public class SwapTransactionRepository : ISwapTransactionService
    {
        private readonly EVSwappingV2Context _context;

        public SwapTransactionRepository(EVSwappingV2Context context) => _context = context;

        public async Task<SwapTransaction?> GetById(long id)
            => await _context.SwapTransactions.FindAsync(id);

        public async Task<List<SwapTransaction>> GetAll()
            => await _context.SwapTransactions.ToListAsync();

        public async Task<List<SwapTransaction>> GetAllWithStationAndReversation()
            => await _context.SwapTransactions.Include(x => x.Reservation).Include(x => x.Station).Include(x => x.CustomerUser).Include(x => x.StaffUser).ToListAsync();

        public async Task<SwapTransaction?> GetAllWithStationAndReversationID(int id)
            => await _context.SwapTransactions.Include(x => x.Reservation).Include(x => x.Station).Include(x => x.CustomerUser).Include(x => x.StaffUser).FirstOrDefaultAsync(x => x.SwapTransactionId == id);

        public async Task Add(SwapTransaction transaction)
        {
            _context.SwapTransactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task Update(SwapTransaction transaction)
        {
            _context.SwapTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public void ConfirmSwapTransaction(long transactionId)
        {
            var transaction = _context.SwapTransactions.FirstOrDefault(t => t.SwapTransactionId == transactionId);
            if (transaction == null) throw new Exception("Transaction not found");

            transaction.SwapStatus = "Confirmed";
            transaction.SwapFinishedAt = DateTime.UtcNow;

            _context.SaveChanges();
        }

    }

}
