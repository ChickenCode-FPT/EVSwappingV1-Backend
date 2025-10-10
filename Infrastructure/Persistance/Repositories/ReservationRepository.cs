using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly EVSwappingV2Context _context;

        public ReservationRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<Reservation?> GetById(int reservationId)
        {
            return await _context.Reservations
                .Include(r => r.Station)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);
        }

        public async Task<IEnumerable<Reservation>> GetByUserId(string userId)
        {
            return await _context.Reservations
                .Include(r => r.Station)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByStationId(int stationId)
        {
            return await _context.Reservations
                .Where(r => r.StationId == stationId)
                .ToListAsync();
        }

        public async Task Add(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task Cancel(int reservationId)
        {
            var res = await _context.Reservations.FindAsync(reservationId);
            if (res != null)
            {
                res.Status = ReservationStatus.Cancelled;
                res.UpdatedAt = DateTime.UtcNow;
                _context.Reservations.Update(res);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Reservation>> GetPendingReservations()
        {
            return await _context.Reservations
                .Where(r => r.Status == ReservationStatus.Pending)
                .ToListAsync();
        }

        public async Task UpdateStatus(int reservationId, string newStatus)
        {
            var res = await _context.Reservations.FindAsync(reservationId);
            if (res != null)
            {
                res.Status = newStatus;
                res.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveChanges() => await _context.SaveChangesAsync();
    }
}
