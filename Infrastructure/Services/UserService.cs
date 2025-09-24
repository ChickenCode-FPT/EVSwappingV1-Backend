using Application.Common.Interfaces;
using Domain.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {

        public readonly UserManager<User> _userManager;
        public readonly IConfiguration _config;
        private readonly EmailService _emailService;

        public UserService(UserManager<User> userManager, IConfiguration config, EmailService emailService)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
        }
        public async Task<List<string>> GetAllUserAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Select(u => u.Email ?? string.Empty).ToList();
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


    }
}
