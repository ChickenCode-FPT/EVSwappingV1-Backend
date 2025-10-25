using MediatR;

namespace Application.Batteries.Commands
{
    public record CreateBatteryCommand(int modelId, int Capacity) : IRequest<int>;
}
