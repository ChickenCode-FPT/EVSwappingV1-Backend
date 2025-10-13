using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdWithDetailsAsync(string userId);
    }
}
