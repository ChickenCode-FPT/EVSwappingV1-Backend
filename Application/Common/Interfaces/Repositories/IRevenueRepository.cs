
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IRevenueRepository
    {
        Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<DateTime, decimal>> GetRevenuePerDayAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, decimal>> GetRevenuePerMonthAsync(DateTime startDate, DateTime endDate);
    }
}
