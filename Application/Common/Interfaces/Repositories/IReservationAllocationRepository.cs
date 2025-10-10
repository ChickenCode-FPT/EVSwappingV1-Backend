using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IReservationAllocationRepository
    {
        Task Add(ReservationAllocation allocation);
        Task<ReservationAllocation?> GetActiveByBattery(int batteryId, DateTime fromUtc, DateTime toUtc);
        Task<IEnumerable<ReservationAllocation>> GetByReservationId(int reservationId);
        Task ReleaseByReservation(int reservationId, string reason);
        Task<int> CountHeldInWindow(int stationId, int batteryModelId, DateTime fromUtc, DateTime toUtc);
        Task<List<ReservationAllocation>> GetExpiredAllocations(DateTime nowUtc);
        Task SaveChanges();
    }
}
