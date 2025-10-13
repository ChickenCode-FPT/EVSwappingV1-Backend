using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EVSwappingV2Context _context;
        public UserRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdWithDetailsAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.Driver)
                .Include(u => u.Vehicles)
                .ThenInclude(v => v.BatteryModelPreference)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
