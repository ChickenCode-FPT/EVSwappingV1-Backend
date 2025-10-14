namespace Application.Dtos
{
    public class BatteryDto
    {
        public int BatteryId { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal? CurrentSoH { get; set; }
        public int? CycleCount { get; set; }
        public int BatteryModelId { get; set; }
    }
}
