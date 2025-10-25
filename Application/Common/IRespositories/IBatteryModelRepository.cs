using Domain.Models;

namespace Application.Common.IRespositories
{
    public interface IBatteryModelRepository
    {
        Task<List<BatteryModel>> GetAll();
        Task<BatteryModel?> GetById(int id);
        Task Add(BatteryModel model);
        Task Update(BatteryModel model);
        Task Delete(int id);
    }
}
