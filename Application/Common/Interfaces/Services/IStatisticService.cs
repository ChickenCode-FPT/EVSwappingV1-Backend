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
        Task<Dictionary<DateTime, int>> GetSwapCountPerDayAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, int>> GetSwapCountPerMonthAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, int>> GetSwapCountPerQuarterAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, int>> GetSwapCountPerYearAsync(DateTime startDate, DateTime endDate);
    }
}
