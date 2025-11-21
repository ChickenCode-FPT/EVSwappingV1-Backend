using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface ISwapTransactionRepository
    {
        Task<bool> ExistsByReservationId(int reservationId);
        Task<SwapTransaction?> GetById(long swapTransactionId);
        Task<IEnumerable<SwapTransaction>> GetByUserId(string userId);
        Task<IEnumerable<SwapTransaction>> GetByStationId(int stationId);
        Task Add(SwapTransaction transaction);
        Task Update(SwapTransaction transaction);
        Task Delete(long swapTransactionId);

        Task<List<SwapTransaction>> GetAll();
        Task<List<SwapTransaction>> GetAllWithStationAndReversation();
        Task<SwapTransaction?> GetAllWithStationAndReversationID(int id);

        Task<int> GetSwapCountAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, int>> GetPeakHoursAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<DateTime, int>> GetSwapCountPerDayAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, int>> GetSwapCountPerMonthAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, int>> GetSwapCountPerQuarterAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, int>> GetSwapCountPerYearAsync(DateTime startDate, DateTime endDate);
    }
}
