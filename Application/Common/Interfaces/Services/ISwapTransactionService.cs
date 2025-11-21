using Application.Dtos.Payment;
using Application.Dtos.Swap;
using Domain.Models;

namespace Application.Common.Interfaces.Services
{
    public interface ISwapTransactionService
    {
        Task<SwapTransaction?> GetById(long id);
        Task<List<SwapTransaction>> GetAll();
        Task Add(SwapTransaction transaction);
        Task Update(SwapTransaction transaction);
        Task<List<SwapTransaction>> GetAllWithStationAndReversation();
        Task<SwapTransaction?> GetAllWithStationAndReversationID(int id);
        Task<IEnumerable<SwapTransactionDto2>> GetAll2();
        Task<SwapTransactionDto2?> GetById2(long id);
        Task<IEnumerable<SwapTransactionDto2>> GetByUser(string userId);
        Task<SwapTransactionDto2> CreateSwap(CreateSwapTransactionRequest request);
        Task<SwapTransactionDto2> CompleteSwap(CompleteSwapTransactionRequest request);
        Task<bool> DeleteSwap(long id);
        Task<PaymentResponseDto> HandleSwapPayment(long swapTransactionId);
        Task<SwapTransactionDto2> ConfirmSwapByStaff(ConfirmSwapByStaffRequest request);
    }
}
