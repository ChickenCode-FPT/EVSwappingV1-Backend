using Application.Common.Interfaces;
using Application.Common.IRespositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories
{
    public class BatteryModelRepository : IBatteryModelRepository
    {
        private readonly EVSwappingV2Context _context;

        public BatteryModelRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<List<BatteryModel>> GetAll()
        {
            return await _context.BatteryModels.ToListAsync();
        }

        public async Task<BatteryModel?> GetById(int id)
        {
            return await _context.BatteryModels.FindAsync(id);
        }

        public async Task Add(BatteryModel model)
        {
            _context.BatteryModels.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(BatteryModel model)
        {
            _context.BatteryModels.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _context.BatteryModels.FindAsync(id);
            if (entity != null)
            {
                _context.BatteryModels.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
