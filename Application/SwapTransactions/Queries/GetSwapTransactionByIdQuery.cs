namespace Application.SwapTransactions.Queries
{
    //public record GetSwapTransactionByIdQuery(long SwapTransactionId) : IRequest<SwapTransaction>;

    //public class GetSwapTransactionByIdQueryHandler : IRequestHandler<GetSwapTransactionByIdQuery, SwapTransaction>
    //{
    //    private readonly ISwapTransactionService _swapTransactionService;

    //    public GetSwapTransactionByIdQueryHandler(ISwapTransactionService swapTransactionService)
    //    {
    //        _swapTransactionService = swapTransactionService;
    //    }

    //    public async Task<SwapTransaction> Handle(GetSwapTransactionByIdQuery request, CancellationToken cancellationToken)
    //    {
    //        var transaction = await _swapTransactionService.GetById(request.SwapTransactionId);

    //        if (transaction == null)
    //        {
    //            throw new Exception("Swap transaction not found.");
    //        }

    //        return transaction;
    //    }
    //}
}
