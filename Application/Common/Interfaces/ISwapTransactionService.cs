using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ISwapTransactionService
    {
        Task<SwapTransaction?> GetById(long id);
        Task<List<SwapTransaction>> GetAll();
        Task Add(SwapTransaction transaction);
        Task Update(SwapTransaction transaction);
        Task<List<SwapTransaction>> GetAllWithStationAndReversation();
        Task<SwapTransaction?> GetAllWithStationAndReversationID(int id);
    }

}
