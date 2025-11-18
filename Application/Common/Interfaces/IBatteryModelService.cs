using Application.Dtos.Battery;
using Domain.Models;

namespace Application.Common.Interfaces
{
    public interface IBatteryModelService
    {
        Task<IEnumerable<BatteryModelDto>> GetAll();
        Task<BatteryModel?> GetById(int id);
        Task Add(BatteryModel model);
        Task Update(BatteryModel model);
        Task Delete(int id);
    }
}
