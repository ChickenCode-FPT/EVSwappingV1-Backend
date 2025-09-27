using Domain.Dtos;

namespace Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUserDtoAsync();
        Task<List<string>> GetAllUserAsync();
    }
}
