using Domain.Dtos;

namespace Application.Common.Interfaces.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUserDtoAsync();
        Task<List<string>> GetAllUserAsync();
        Task PromoteUserRoleAsync(string userId, string newRole, string changedByUserId, bool replaceExisting = true);
    }
}
