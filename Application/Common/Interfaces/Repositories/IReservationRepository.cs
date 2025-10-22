using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetById(int reservationId);
        Task<IEnumerable<Reservation>> GetByUserId(string userId);
        Task<IEnumerable<Reservation>> GetByStationId(int stationId);
        Task Add(Reservation reservation);
        Task Update(Reservation reservation);
        Task Cancel(int reservationId);
    }
}
