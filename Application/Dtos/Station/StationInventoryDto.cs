using Application.Dtos.Battery;

namespace Application.Dtos.Station
{
    public class StationInventoryDto
    {
        public int StationId { get; set; }
        public int FullBatteries { get; set; }
        public int ChargingBatteries { get; set; }
        public int MaintenanceBatteries { get; set; }
        public List<BatteryDetailDto> Batteries { get; set; } = new();
    }
}
