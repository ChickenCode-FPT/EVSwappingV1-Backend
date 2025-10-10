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
    }
}
