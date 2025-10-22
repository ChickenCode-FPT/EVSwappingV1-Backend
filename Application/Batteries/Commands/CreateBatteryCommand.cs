using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Batteries.Commands
{
    public record CreateBatteryCommand(int modelId, int Capacity, BatteryStatus Status) : IRequest<int>;
}
