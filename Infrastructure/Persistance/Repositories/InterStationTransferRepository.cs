using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class InterStationTransferRepository : IInterStationTransferRepository
    {
        private readonly EVSwappingV2Context _context;
        public InterStationTransferRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<InterStationTransfer> CreateAsync(InterStationTransfer transfer)
        {
            _context.InterStationTransfers.Add(transfer);
            await _context.SaveChangesAsync();
            return transfer;
        }

        public async Task<InterStationTransfer?> GetByIdAsync(long id)
        {
            return await _context.InterStationTransfers
                .Include(t => t.FromStation)
                .Include(t => t.ToStation)
                .Include(t => t.Battery)
                .Include(t => t.RequestedByUser)
                .Include(t => t.ApprovedByUser)
                .FirstOrDefaultAsync(t => t.TransferId == id);
        }

        public async Task<IEnumerable<InterStationTransfer>> GetByStationAsync(int stationId)
        {
            return await _context.InterStationTransfers
                .Include(t => t.Battery)
                .Include(t => t.FromStation)
                .Include(t => t.ToStation)
                .Include(t => t.RequestedByUser)
                .Include(t => t.ApprovedByUser)
                .Where(t => t.FromStationId == stationId || t.ToStationId == stationId)
                .OrderByDescending(t => t.RequestedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(InterStationTransfer transfer)
        {
            _context.InterStationTransfers.Update(transfer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InterStationTransfer>> GetOutgoingTransfersAsync(int stationId)
        {
            return await _context.InterStationTransfers
                .Include(t => t.FromStation)
                .Include(t => t.ToStation)
                .Include(t => t.Battery)
                .Include(t => t.RequestedByUser)
                .Include(t => t.ApprovedByUser)
                .Where(t => t.FromStationId == stationId)
                .OrderByDescending(t => t.RequestedAt)
                .ToListAsync();
        }

        public async Task<List<InterStationTransfer>> GetIncomingTransfersAsync(int stationId)
        {
            return await _context.InterStationTransfers
                .Include(t => t.FromStation)
                .Include(t => t.ToStation)
                .Include(t => t.Battery)
                .Include(t => t.RequestedByUser)
                .Include(t => t.ApprovedByUser)
                .Where(t => t.ToStationId == stationId)
                .OrderByDescending(t => t.RequestedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<InterStationTransfer>> GetAllTransfersAsync()
        {
            return await _context.InterStationTransfers
                .Include(t => t.RequestedByUser)
                .Include(t => t.ApprovedByUser)
                .Include(t => t.FromStation)
                .Include(t => t.ToStation)
                .OrderByDescending(t => t.RequestedAt)
                .ToListAsync();
        }

    }
}
