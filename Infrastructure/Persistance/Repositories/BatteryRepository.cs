using Application.Common.Interfaces;
using Domain.Enums;
using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories
{
    public class BatteryRepository : IBatteryService
    public class BatteryRepository : IBatteryRepository
    {
        private readonly EVSwappingV2Context _context;

        public BatteryRepository(EVSwappingV2Context context) => _context = context;

        public async Task<Battery?> GetById(int id) 
            => await _context.Batteries.Include(x => x.BatteryModel).FirstOrDefaultAsync(b => b.BatteryId == id);

        public async Task<List<Battery>> GetAll() 
        public BatteryRepository(EVSwappingV2Context context)
        {
            return await _context.Batteries.Include(x => x.BatteryModel).ToListAsync();
            _context = context;
        }

        public async Task<List<Battery>> GetByStatus(string status)
            => await _context.Batteries.Where(x => x.Status == status).ToListAsync();

        public async Task<IEnumerable<Battery>> GetBatteries(decimal? capacity = null, int? modelId = null, string? status = null)
        public async Task<Battery?> GetById(int batteryId)
        {
            var query = _context.Batteries.AsQueryable();
            return await _context.Batteries.FindAsync(batteryId);
        }

            if (capacity != null)
                query = query.Where(b => b.BatteryModel.CapacityKwh == capacity);

            if (modelId.HasValue)
                query = query.Where(b => b.BatteryModelId == modelId.Value);
        public async Task<IEnumerable<Battery>> GetAvailableBatteries(int? batteryModelId = null)
        {
            var query = _context.Batteries.Where(b => b.Status == "Available");

            if (!string.IsNullOrEmpty(status))
                query = query.Where(b => b.Status == status);
            if (batteryModelId.HasValue)
                query = query.Where(b => b.BatteryModelId == batteryModelId.Value);

            return await query.ToListAsync();
        }


        public async Task Add(Battery battery)
        public async Task UpdateStatus(int batteryId, string status)
        {
            var battery = await _context.Batteries.FindAsync(batteryId);
            if (battery != null)
            {
            _context.Batteries.Add(battery);
                battery.Status = status;
                _context.Batteries.Update(battery);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Update(Battery battery)
        public async Task Add(Battery battery)
        {
            _context.Batteries.Update(battery);
            await _context.Batteries.AddAsync(battery);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _context.Batteries.FindAsync(id);
            if (entity != null)
        public async Task Update(Battery battery)
        {
                _context.Batteries.Remove(entity);
            _context.Batteries.Update(battery);
            await _context.SaveChangesAsync();
        }
    }
}
}
