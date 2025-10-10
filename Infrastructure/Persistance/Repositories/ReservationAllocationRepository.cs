using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class ReservationAllocationRepository : IReservationAllocationRepository
    {
        private readonly EVSwappingV2Context _context;

        public ReservationAllocationRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task Add(ReservationAllocation allocation)
        {
            _context.ReservationAllocations.Add(allocation);
            await _context.SaveChangesAsync();
        }

        public async Task<ReservationAllocation?> GetActiveByBattery(int batteryId, DateTime fromUtc, DateTime toUtc)
        {
            return await _context.ReservationAllocations
                .FirstOrDefaultAsync(a =>
                    a.BatteryId == batteryId &&
                    a.Status == "Active" &&
                    a.HoldUntil > fromUtc &&
                    a.Reservation.ReservedFrom < toUtc);
        }

        public async Task<IEnumerable<ReservationAllocation>> GetByReservationId(int reservationId)
        {
            return await _context.ReservationAllocations
                .Where(a => a.ReservationId == reservationId)
                .ToListAsync();
        }

        public async Task ReleaseByReservation(int reservationId, string reason)
        {
            var allocations = await _context.ReservationAllocations
                .Where(a => a.ReservationId == reservationId && a.Status == "Active")
                .ToListAsync();

            foreach (var alloc in allocations)
            {
                alloc.Status = reason switch
                {
                    "Cancelled" => "Released",
                    "Expired" => "Expired",
                    _ => "Released"
                };
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> CountHeldInWindow(int stationId, int batteryModelId, DateTime fromUtc, DateTime toUtc)
        {
            return await _context.ReservationAllocations
                .Where(a => a.Status == "Active"
                            && a.Battery.BatteryModelId == batteryModelId
                            && a.Battery.StationInventories.Any(si => si.StationId == stationId)
                            && a.HoldUntil > fromUtc
                            && a.Reservation.ReservedFrom < toUtc)
                .CountAsync();
        }

        public async Task<List<ReservationAllocation>> GetExpiredAllocations(DateTime nowUtc)
        {
            return await _context.ReservationAllocations
                .Include(a => a.Reservation)
                .ThenInclude(r => r.Station)
                .Where(a => a.Status == "Active" && a.HoldUntil < nowUtc)
                .ToListAsync();
        }

        public async Task SaveChanges() => await _context.SaveChangesAsync();
    }
}
