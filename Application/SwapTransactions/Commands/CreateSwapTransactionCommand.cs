using MediatR;

namespace Application.SwapTransactions.Commands
{
    public record CreateSwapTransactionCommand(string StaffId, string CustomerId, int OldBatteryId, int NewBatteryId, decimal Fee) : IRequest<int>;
}
