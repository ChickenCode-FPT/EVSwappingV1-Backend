using Application.Common.Interfaces;
using Domain.Enums;
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
    {
        private readonly EVSwappingV2Context _context;

        public BatteryRepository(EVSwappingV2Context context) => _context = context;

        public async Task<Battery?> GetById(int id) 
            => await _context.Batteries.Include(x => x.BatteryModel).FirstOrDefaultAsync(b => b.BatteryId == id);

        public async Task<List<Battery>> GetAll() 
        {
            return await _context.Batteries.Include(x => x.BatteryModel).ToListAsync();
        } 

        public async Task<List<Battery>> GetByStatus(string status)
            => await _context.Batteries.Where(x => x.Status == status).ToListAsync();

        public async Task<IEnumerable<Battery>> GetBatteries(decimal? capacity = null, int? modelId = null, string? status = null)
        {
            var query = _context.Batteries.AsQueryable();

            if (capacity != null)
                query = query.Where(b => b.BatteryModel.CapacityKwh == capacity);

            if (modelId.HasValue)
                query = query.Where(b => b.BatteryModelId == modelId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(b => b.Status == status);

            return await query.ToListAsync();
        }


        public async Task Add(Battery battery)
        {
            _context.Batteries.Add(battery);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Battery battery)
        {
            _context.Batteries.Update(battery);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _context.Batteries.FindAsync(id);
            if (entity != null)
            {
                _context.Batteries.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
