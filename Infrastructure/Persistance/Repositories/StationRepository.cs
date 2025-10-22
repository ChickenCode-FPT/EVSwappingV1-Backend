using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;

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
            return await _context.Stations.FindAsync(stationId);
        }

        public async Task<IEnumerable<Station>> GetAll()
        {
            return await _context.Stations.ToListAsync();
        }

        public async Task<IEnumerable<Station>> GetNearbyStations(decimal latitude, decimal longitude, double radiusKm)
        {
            return await _context.Stations
                .Where(s => s.Latitude != null && s.Longitude != null)
                .ToListAsync();
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
