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

        /// <summary>
        /// Kiểm tra xem Reservation có giao dịch đổi pin nào chưa.
        /// </summary>
        public async Task<bool> ExistsByReservationId(int reservationId)
        {
            return await _context.SwapTransactions
                .AnyAsync(t => t.ReservationId == reservationId && t.SwapStatus == "Completed");
        }

        public async Task<SwapTransaction?> GetById(long swapTransactionId)
        {
            return await _context.SwapTransactions
                .Include(t => t.Reservation)
                .Include(t => t.Station)
                .Include(t => t.CustomerUser)
                .FirstOrDefaultAsync(t => t.SwapTransactionId == swapTransactionId);
        }

        public async Task<IEnumerable<SwapTransaction>> GetByUserId(string userId)
        {
            return await _context.SwapTransactions
                .Include(t => t.Station)
                .Where(t => t.CustomerUserId == userId)
                .OrderByDescending(t => t.SwapStartedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SwapTransaction>> GetByStationId(int stationId)
        {
            return await _context.SwapTransactions
                .Include(t => t.CustomerUser)
                .Where(t => t.StationId == stationId)
                .OrderByDescending(t => t.SwapStartedAt)
                .ToListAsync();
        }

        public async Task Add(SwapTransaction transaction)
        {
            await _context.SwapTransactions.AddAsync(transaction);
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
    }
}
