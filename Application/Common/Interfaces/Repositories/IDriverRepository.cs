using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IDriverRepository
    {
        Task<Driver?> GetByUserIdAsync(string userId);
        Task<Driver> AddAsync(Driver driver);
    }
}
