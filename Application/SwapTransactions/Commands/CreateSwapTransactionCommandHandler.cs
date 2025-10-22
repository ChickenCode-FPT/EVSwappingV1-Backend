namespace Application.SwapTransactions.Commands
{
    //public class CreateSwapTransactionCommandHandler : IRequestHandler<CreateSwapTransactionCommand, int>
    //{
    //    private readonly ISwapTransactionService _repo;

    //    public CreateSwapTransactionCommandHandler(ISwapTransactionService repo) => _repo = repo;

    //    public async Task<int> Handle(CreateSwapTransactionCommand request, CancellationToken cancellationToken)
    //    {
    //        var tx = new SwapTransaction
    //        {
    //            StaffUserId = request.StaffId,
    //            CustomerUserId = request.CustomerId,
    //            OutgoingBatteryId = request.OldBatteryId,
    //            IncomingBatteryId = request.NewBatteryId,
    //            Price = request.Fee,
    //            SwapStatus = PaymentStatus.Pending.ToString(),
    //        };

    //        await _repo.Add(tx);
    //        return tx.OutgoingBatteryId.Value;
    //    }
    //}
}
