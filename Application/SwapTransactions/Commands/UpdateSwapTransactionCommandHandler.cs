namespace Application.SwapTransactions.Commands
{
    //// Lệnh yêu cầu cập nhật giao dịch swap
    //public record UpdateSwapTransactionCommand : IRequest<int>
    //{
    //    public long SwapTransactionId { get; init; }
    //    public string CustomerId { get; init; }
    //    public string? StaffId { get; init; }
    //    public int? OldBatteryId { get; init; }
    //    public int? NewBatteryId { get; init; }
    //    public decimal Fee { get; init; }
    //    public string? SwapStatus { get; init; }
    //}

    //// Xử lý lệnh cập nhật giao dịch swap
    //public class UpdateSwapTransactionCommandHandler : IRequestHandler<UpdateSwapTransactionCommand, int>
    //{
    //    private readonly ISwapTransactionService _repo;
    //    private readonly IMapper _mapper;

    //    public UpdateSwapTransactionCommandHandler(ISwapTransactionService repo, IMapper mapper)
    //    {
    //        _repo = repo;
    //        _mapper = mapper;
    //    }

    //    public async Task<int> Handle(UpdateSwapTransactionCommand request, CancellationToken cancellationToken)
    //    {
    //        var tx = await _repo.GetById(request.SwapTransactionId);

    //        if (tx == null)
    //        {
    //            throw new Exception("Swap transaction not found.");
    //        }

    //        _mapper.Map(request, tx);

    //        await _repo.Update(tx);

    //        return tx.OutgoingBatteryId.Value;
    //    }
    //}
}
