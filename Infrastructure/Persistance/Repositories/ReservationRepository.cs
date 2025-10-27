using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistance.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly EVSwappingV2Context _context;

        public ReservationRepository(EVSwappingV2Context context)
        {
            _context = context;
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

        public async Task Delete(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
                throw new KeyNotFoundException($"Reservation with id={reservationId} not found.");

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Reservation?> GetById(int reservationId)
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Station)
                .Include(r => r.Vehicle)
                .Include(r => r.Payments)
                .Include(r => r.ReservationAllocations)
                    .ThenInclude(a => a.Battery)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);
        }

        public async Task<IEnumerable<Reservation>> GetAll()
        {
            return await _context.Reservations
                .Include(r => r.Station)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByUserId(string userId)
        {
            return await _context.Reservations
                .Include(r => r.Station)
                .Include(r => r.ReservationAllocations)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByStationId(int stationId)
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Where(r => r.StationId == stationId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task Cancel(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
                throw new KeyNotFoundException($"Reservation with id={reservationId} not found.");

            reservation.Status = ReservationStatus.Cancelled;
            reservation.UpdatedAt = DateTime.UtcNow;

            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatus(int reservationId, string newStatus)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
                throw new KeyNotFoundException($"Reservation with id={reservationId} not found.");

            reservation.Status = newStatus;
            reservation.UpdatedAt = DateTime.UtcNow;
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Reservation>> GetPendingReservations()
        {
            return await _context.Reservations
                .Include(r => r.Station)
                .Where(r => r.Status == ReservationStatus.Pending)
                .OrderBy(r => r.ReservedFrom)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetActiveReservations()
        {
            return await _context.Reservations
                .Include(r => r.Station)
                .Where(r =>
                    r.Status == ReservationStatus.Pending ||
                    r.Status == ReservationStatus.Completed &&
                    (r.ReservedTo > DateTime.UtcNow))
                .OrderBy(r => r.ReservedFrom)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetExpiredUnprocessed()
        {
            return await _context.Reservations
                .Where(r =>
                    r.Status == ReservationStatus.Pending &&
                    r.ReservedTo < DateTime.UtcNow)
                .OrderBy(r => r.ReservedTo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetCompletedReservations()
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Where(r => r.Status == ReservationStatus.Completed)
                .OrderByDescending(r => r.UpdatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByDateRange(DateTime start, DateTime end)
        {
            return await _context.Reservations
                .Include(r => r.Station)
                .Include(r => r.User)
                .Where(r => r.CreatedAt >= start && r.CreatedAt <= end)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        }
    }
}
