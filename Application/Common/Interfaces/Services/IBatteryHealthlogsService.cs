using Application.Dtos;

namespace Application.Common.Interfaces.Services
{
    public interface IBatteryHealthlogsService
    {
        Task<IEnumerable<BatteryHealthLogsDto>> GetAll();
        Task<bool> AddAsync(CreateBatteryHealthLogDto dto);
        Task<BatteryHealthLogsDto> UpdateAsync(int id, UpdateBatteryHealthLogDto dto);
        Task<bool> DeleteAsync(int id);

    }
}
