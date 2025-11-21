using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IRevenueRepository _revenueRepository;
        private readonly ISwapTransactionRepository _swapTransactionRepository;

        public StatisticService(IRevenueRepository revenueRepository, ISwapTransactionRepository swapTransactionRepository)
        {
            _revenueRepository = revenueRepository;
            _swapTransactionRepository = swapTransactionRepository;
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            return await _revenueRepository.GetTotalRevenueAsync(startDate, endDate);
        }

        public async Task<Dictionary<DateTime, decimal>> GetRevenuePerDayAsync(DateTime startDate, DateTime endDate)
        {
            return await _revenueRepository.GetRevenuePerDayAsync(startDate, endDate);
        }

        public async Task<Dictionary<int, decimal>> GetRevenuePerMonthAsync(DateTime startDate, DateTime endDate)
        {
            return await _revenueRepository.GetRevenuePerMonthAsync(startDate, endDate);
        }

        public async Task<int> GetSwapCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _swapTransactionRepository.GetSwapCountAsync(startDate, endDate);
        }

        public async Task<Dictionary<int, int>> GetPeakHoursAsync(DateTime startDate, DateTime endDate)
        {
            return await _swapTransactionRepository.GetPeakHoursAsync(startDate, endDate);
        }

        public async Task<Dictionary<DateTime, int>> GetSwapCountPerDayAsync(DateTime startDate, DateTime endDate)
        {
            return await _swapTransactionRepository.GetSwapCountPerDayAsync(startDate, endDate);
        }

        public async Task<Dictionary<int, int>> GetSwapCountPerMonthAsync(DateTime startDate, DateTime endDate)
        {
            return await _swapTransactionRepository.GetSwapCountPerMonthAsync(startDate, endDate);
        }

        public async Task<Dictionary<int, int>> GetSwapCountPerQuarterAsync(DateTime startDate, DateTime endDate)
        {
            return await _swapTransactionRepository.GetSwapCountPerQuarterAsync(startDate, endDate);
        }

        public async Task<Dictionary<int, int>> GetSwapCountPerYearAsync(DateTime startDate, DateTime endDate)
        {
            return await _swapTransactionRepository.GetSwapCountPerYearAsync(startDate, endDate);
        }
    }
}
