using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly EVSwappingV2Context _context;

        public DriverRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<Driver?> GetByUserId(string userId)
        {
            return await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<Driver> Add(Driver driver)
        {
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();
            return driver;
        }
    }
}
