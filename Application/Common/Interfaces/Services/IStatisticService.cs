using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Services
{
    public interface IStatisticService
    {
        Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<DateTime, decimal>> GetRevenuePerDayAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, decimal>> GetRevenuePerMonthAsync(DateTime startDate, DateTime endDate);
        Task<int> GetSwapCountAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, int>> GetPeakHoursAsync(DateTime startDate, DateTime endDate);
    }
}
