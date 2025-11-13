using Application.Dtos.Battery;

namespace Application.Dtos.Station
{
    public class StationInventoryDto
    {
        public int StationInventoryId { get; set; }

        public int StationId { get; set; }

        public int BatteryId { get; set; }

        public string SlotNumber { get; set; }

        public string Status { get; set; }

        public DateTime? CheckedAt { get; set; }

        public BatteriesDto Batteries { get; set; }
    }
}
