using MediatR;

namespace Application.Batteries.Commands
{
    public record UpdateBatteryStatusCommand(int Id) : IRequest;
}
