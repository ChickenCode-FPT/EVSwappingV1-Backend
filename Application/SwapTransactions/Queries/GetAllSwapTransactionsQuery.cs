using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.SwapTransactions.Queries
{
    public record GetAllSwapTransactionsQuery() : IRequest<List<SwapTransaction>>;

    public class GetAllSwapTransactionsQueryHandler : IRequestHandler<GetAllSwapTransactionsQuery, List<SwapTransaction>>
    {
        private readonly ISwapTransactionService _swapTransactionService;

        public GetAllSwapTransactionsQueryHandler(ISwapTransactionService swapTransactionService)
        {
            _swapTransactionService = swapTransactionService;
        }

        public async Task<List<SwapTransaction>> Handle(GetAllSwapTransactionsQuery request, CancellationToken cancellationToken)
        {
            // Truy vấn tất cả giao dịch từ repository
            var transactions = await _swapTransactionService.GetAll();

            return transactions;
        }
    }
}
