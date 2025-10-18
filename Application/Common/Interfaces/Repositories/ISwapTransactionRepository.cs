using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface ISwapTransactionRepository
    {
        Task<int> GetSwapCountAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, int>> GetPeakHoursAsync(DateTime startDate, DateTime endDate);
    }
}
