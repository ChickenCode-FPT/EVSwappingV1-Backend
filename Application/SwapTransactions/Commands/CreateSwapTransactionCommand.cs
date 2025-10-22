using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.SwapTransactions.Commands
{
    public record CreateSwapTransactionCommand(string StaffId, string CustomerId, int OldBatteryId, int NewBatteryId, decimal Fee) : IRequest<int>;
}
