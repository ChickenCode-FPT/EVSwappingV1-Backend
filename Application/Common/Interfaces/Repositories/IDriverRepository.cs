using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IDriverRepository
    {
        Task<Driver?> GetByUserId(string userId);
        Task<Driver> Add(Driver driver);
    }
}
