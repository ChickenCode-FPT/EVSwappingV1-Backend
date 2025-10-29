using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class SwapTransactionRepository : ISwapTransactionRepository
    {
        private readonly EVSwappingV2Context _context;

        public SwapTransactionRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByReservationId(int reservationId)
        {
            return await _context.SwapTransactions
                .AnyAsync(t => t.ReservationId == reservationId && t.SwapStatus == "Completed");
        }

        //public async Task<SwapTransaction?> GetById(long id)
        //    => await _context.SwapTransactions.FindAsync(id);

        public async Task<SwapTransaction?> GetById(long swapTransactionId)
        {
            return await _context.SwapTransactions
                .Include(t => t.Reservation)
                .Include(t => t.Station)
                .Include(t => t.CustomerUser)
                .FirstOrDefaultAsync(t => t.SwapTransactionId == swapTransactionId);
        }

        public async Task<List<SwapTransaction>> GetAll()
            => await _context.SwapTransactions.ToListAsync();

        public async Task<IEnumerable<SwapTransaction>> GetByUserId(string userId)
        {
            return await _context.SwapTransactions
                .Include(t => t.Station)
                .Where(t => t.CustomerUserId == userId)
                .OrderByDescending(t => t.SwapStartedAt)
                .ToListAsync();
        }
        public async Task<List<SwapTransaction>> GetAllWithStationAndReversation()
            => await _context.SwapTransactions.Include(x => x.Reservation).Include(x => x.Station).Include(x => x.CustomerUser).Include(x => x.StaffUser).ToListAsync();

        public async Task<IEnumerable<SwapTransaction>> GetByStationId(int stationId)
        {
            return await _context.SwapTransactions
                .Include(t => t.CustomerUser)
                .Where(t => t.StationId == stationId)
                .OrderByDescending(t => t.SwapStartedAt)
                .ToListAsync();
        }
        public async Task<SwapTransaction?> GetAllWithStationAndReversationID(int id)
            => await _context.SwapTransactions.Include(x => x.Reservation).Include(x => x.Station).Include(x => x.CustomerUser).Include(x => x.StaffUser).FirstOrDefaultAsync(x => x.SwapTransactionId == id);

        public async Task Add(SwapTransaction transaction)
        {
            await _context.SwapTransactions.AddAsync(transaction);
            _context.SwapTransactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task Update(SwapTransaction transaction)
        {
            _context.SwapTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(long swapTransactionId)
        {
            var tx = await _context.SwapTransactions.FindAsync(swapTransactionId);
            if (tx != null)
            {
                _context.SwapTransactions.Remove(tx);
                await _context.SaveChangesAsync();
            }
        }

        public void ConfirmSwapTransaction(long transactionId)
        {
            var transaction = _context.SwapTransactions.FirstOrDefault(t => t.SwapTransactionId == transactionId);
            if (transaction == null) throw new Exception("Transaction not found");

            transaction.SwapStatus = "Confirmed";
            transaction.SwapFinishedAt = DateTime.UtcNow;

            _context.SaveChanges();
        }

        public async Task<IEnumerable<SwapTransaction>> GetAll2()
            => await _context.SwapTransactions.AsNoTracking().ToListAsync();

        public async Task<SwapTransaction?> GetById2(long id)
            => await _context.SwapTransactions.FindAsync(id);

        public async Task<IEnumerable<SwapTransaction>> GetByUser(string userId)
            => await _context.SwapTransactions
                .Where(s => s.CustomerUserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

        public async Task<SwapTransaction> Add2(SwapTransaction swap)
        {
            await _context.SwapTransactions.AddAsync(swap);
            await _context.SaveChangesAsync();
            return swap;
        }

        public async Task Update2(SwapTransaction swap)
        {
            _context.SwapTransactions.Update(swap);
            await _context.SaveChangesAsync();
        }

        public async Task Delete2(long id)
        {
            var entity = await _context.SwapTransactions.FindAsync(id);
            if (entity != null)
            {
                _context.SwapTransactions.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByReservationId2(int reservationId)
        { 
            return await _context.SwapTransactions.AnyAsync(s => s.ReservationId == reservationId);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
