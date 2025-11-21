using MediatR;

namespace Application.Batteries.Commands
{
    public record CreateBatteryCommand : IRequest<int>
    {
        public string SerialNumber { get; set; } = string.Empty;

        public int BatteryModelId { get; set; }

        public decimal? CurrentSoH { get; set; }

        public int? CycleCount { get; set; }

        public string Status { get; set; } = string.Empty;

        public int StationId { get; set; }
    }
}
