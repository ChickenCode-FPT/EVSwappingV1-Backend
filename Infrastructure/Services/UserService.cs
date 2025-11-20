using Application.Common.Interfaces.Services;
using Domain.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {

        public readonly UserManager<User> _userManager;
        public readonly IConfiguration _config;
        private readonly EmailService _emailService;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UserService(UserManager<User> userManager, IConfiguration config, EmailService emailService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
            _roleManager = roleManager;
        }
        public async Task<List<string>> GetAllUserAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Select(u => u.Email ?? string.Empty).ToList();
        }

        public async Task<User?> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }

        public async Task<List<UserDto>> GetAllUserDtoAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                    continue;

                userDtos.Add(new UserDto
                {
                    Id = user.Id.ToString(),
                    Email = user.Email ?? "",
                    FullName = user.FullName ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    Roles = roles.ToList(),
                    Lockout = user.LockoutEnabled && user.LockoutEnd > DateTime.UtcNow
                });
            }

            return userDtos;
        }

        public async Task<bool> LockUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UnlockUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            user.LockoutEnd = DateTimeOffset.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        public async Task PromoteUserRoleAsync(string userId, string newRole, string changedByUserId, bool replaceExisting = true)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Kiểm tra role có tồn tại không
            if (!await _roleManager.RoleExistsAsync(newRole))
                await _roleManager.CreateAsync(new IdentityRole(newRole));

            if (replaceExisting)
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, newRole);

        }


    }
}
