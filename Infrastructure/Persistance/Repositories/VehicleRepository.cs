using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly EVSwappingV2Context _context;

        public VehicleRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<Vehicle?> GetById(int vehicleId)
        {
            return await _context.Vehicles
                .Include(v => v.BatteryModelPreference)
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
        }

        public async Task<IEnumerable<Vehicle>> GetByUserId(string userId)
        {
            return await _context.Vehicles.Where(v => v.UserId == userId).ToListAsync();
        }

        public async Task Add(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int vehicleId)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
        }
    }
}
