using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
