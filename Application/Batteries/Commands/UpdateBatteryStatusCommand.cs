using MediatR;

namespace Application.Batteries.Commands
{
    public record UpdateBatteryStatusCommand: IRequest <int>
    {
        public int BatteryId { get; set; }
        public string? Status { get; set; }
    }

}
