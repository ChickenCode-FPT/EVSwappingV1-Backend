using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class StationRepository : IStationRepository
    {
        private readonly EVSwappingV2Context _context;

        public StationRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<Station?> GetById(int stationId)
        {
            return await _context.Stations
                .Include(s => s.StationInventories)
                .FirstOrDefaultAsync(s => s.StationId == stationId);
        }

        public async Task<IEnumerable<Station>> GetAll()
        {
            return await _context.Stations
                .Include(s => s.StationInventories)
                .ToListAsync();
        }

        public async Task<int> CountAvailableBatteries(int stationId)
        {
            return await _context.StationInventories
                .Include(i => i.Battery)
                .CountAsync(i =>
                    i.StationId == stationId &&
                    i.Status == StationInventoryStatus.Full &&
                    i.Battery.Status == BatteryStatus.Full);
        }

        public async Task Add(Station station)
        {
            await _context.Stations.AddAsync(station);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Station station)
        {
            _context.Stations.Update(station);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int stationId)
        {
            var station = await _context.Stations.FindAsync(stationId);
            if (station != null)
            {
                _context.Stations.Remove(station);
                await _context.SaveChangesAsync();
            }
        }
    }
}
