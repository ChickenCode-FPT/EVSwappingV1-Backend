using Domain.Dtos;
using Domain.Models;

namespace Application.Common.Interfaces.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUserDtoAsync();
        Task<List<string>> GetAllUserAsync();
        Task<User?> GetUser(string userId);
        Task PromoteUserRoleAsync(string userId, string newRole, string changedByUserId, bool replaceExisting = true);
    }
}
