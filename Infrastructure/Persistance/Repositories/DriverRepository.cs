using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories
{
    public class DriverRepository //: IDriverRepository
    {
        private readonly EVSwappingV2Context _context;

        public DriverRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<Driver?> GetByUserIdAsync(string userId)
        {
            return await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<Driver> AddAsync(Driver driver)
        {
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();
            return driver;
        }
    }
}