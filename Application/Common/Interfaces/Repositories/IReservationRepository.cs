using Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Common.Interfaces.Repositories
{
    public interface IReservationRepository
    {
        Task Add(Reservation reservation);
        Task Update(Reservation reservation);
        Task Delete(int reservationId);
        Task SaveChanges();
        Task<Reservation?> GetById(int reservationId);
        Task<IEnumerable<Reservation>> GetAll();
        Task<IEnumerable<Reservation>> GetByUserId(string userId);
        Task<IEnumerable<Reservation>> GetByStationId(int stationId);
        Task Cancel(int reservationId);
        Task UpdateStatus(int reservationId, string newStatus);
        Task<IEnumerable<Reservation>> GetPendingReservations();
        Task<IEnumerable<Reservation>> GetActiveReservations();
        Task<IEnumerable<Reservation>> GetExpiredUnprocessed();
        Task<IEnumerable<Reservation>> GetCompletedReservations();
        Task<IEnumerable<Reservation>> GetByDateRange(DateTime start, DateTime end);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<IEnumerable<Reservation>> GetByStatus(string status);
        Task<IEnumerable<Reservation>> GetPendingReservationsBetween(DateTime start, DateTime end);
        Task<bool> HasOverlappingReservation(string userId, DateTime fromUtc, DateTime toUtc);
    }
}
