using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EVSwappingV2Context _context;
        public readonly UserManager<User> _userManager;

        public UserRepository(EVSwappingV2Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<User?> GetByIdWithDetailsAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.Driver)
                .Include(u => u.Vehicles)
                .ThenInclude(v => v.BatteryModelPreference)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

    }
}
