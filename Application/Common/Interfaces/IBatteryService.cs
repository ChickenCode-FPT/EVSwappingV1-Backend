using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IBatteryService
    {
        Task<Battery?> GetById(int id);
        Task<List<Battery>> GetAll();
        Task<List<Battery>> GetByStatus(string status);
        Task Add(Battery battery);
        Task Update(Battery battery);
        Task Delete(int id);
        Task<IEnumerable<Battery>> GetBatteries(decimal? capacity = null, int? modelId = null, string? status = null);
    }

}
