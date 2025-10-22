using Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
