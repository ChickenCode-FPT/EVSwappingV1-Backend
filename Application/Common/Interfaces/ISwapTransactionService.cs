using Domain.Models;

namespace Application.Common.Interfaces
{
    public interface ISwapTransactionRepositoryISwapTransactionService
    {
        Task<SwapTransaction?> GetById(long id);
        Task<List<SwapTransaction>> GetAll();
        Task Add(SwapTransaction transaction);
        Task Update(SwapTransaction transaction);
        Task<List<SwapTransaction>> GetAllWithStationAndReversation();
        Task<SwapTransaction?> GetAllWithStationAndReversationID(int id);
    }

}
