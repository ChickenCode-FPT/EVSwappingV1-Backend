using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using MediatR;

namespace Application.SwapTransactions.Commands
{
    public class CreateSwapTransactionCommandHandler : IRequestHandler<CreateSwapTransactionCommand, int>
    {
        private readonly ISwapTransactionRepository _repo;

        public CreateSwapTransactionCommandHandler(ISwapTransactionRepository repo) => _repo = repo;

        public async Task<int> Handle(CreateSwapTransactionCommand request, CancellationToken cancellationToken)
        {
            var tx = new SwapTransaction
            {
                StaffUserId = request.StaffId,
                CustomerUserId = request.CustomerId,
                OutgoingBatteryId = request.OldBatteryId,
                IncomingBatteryId = request.NewBatteryId,
                Price = request.Fee,
                SwapStatus = PaymentStatus.Pending.ToString(),
            };

            await _repo.Add(tx);
            return tx.OutgoingBatteryId.Value;
        }
    }
}
