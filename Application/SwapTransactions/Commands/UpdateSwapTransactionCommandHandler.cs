using Application.Common.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Application.SwapTransactions.Commands
{
    // Lệnh yêu cầu cập nhật giao dịch swap
    public record UpdateSwapTransactionCommand : IRequest<int>
    {
        public long SwapTransactionId { get; init; }
        public string CustomerId { get; init; }
        public string? StaffId { get; init; }
        public decimal Fee { get; init; }
        public string? SwapStatus { get; init; }
    }

    public class UpdateSwapTransactionCommandHandler : IRequestHandler<UpdateSwapTransactionCommand, int>
    {
        private readonly ISwapTransactionRepository _repo;
        private readonly IMapper _mapper;

        public UpdateSwapTransactionCommandHandler(ISwapTransactionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<int> Handle(UpdateSwapTransactionCommand request, CancellationToken cancellationToken)
        {
            var tx = await _repo.GetById(request.SwapTransactionId);

            if (tx == null)
            {
                throw new Exception("Swap transaction not found.");
            }

            _mapper.Map(request, tx);

            await _repo.Update(tx);

            return tx.OutgoingBatteryId.Value;
        }
    }
}
