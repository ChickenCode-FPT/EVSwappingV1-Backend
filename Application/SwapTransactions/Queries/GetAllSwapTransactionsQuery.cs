using Application.Common.Interfaces.Repositories;
using Domain.Models;
using MediatR;

namespace Application.SwapTransactions.Queries
{
    public record GetAllSwapTransactionsQuery() : IRequest<List<SwapTransaction>>;

    public class GetAllSwapTransactionsQueryHandler : IRequestHandler<GetAllSwapTransactionsQuery, List<SwapTransaction>>
    {
        private readonly ISwapTransactionRepository _swapTransactionService;

        public GetAllSwapTransactionsQueryHandler(ISwapTransactionRepository swapTransactionService)
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
