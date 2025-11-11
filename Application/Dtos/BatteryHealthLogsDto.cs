namespace Application.Dtos
{
    public class BatteryHealthLogsDto
    {
        public long BatteryHealthLogId { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public int BatteryId { get; set; }
        public DateTime RecordedAt { get; set; }
        public decimal? SoH { get; set; }
        public int? CycleCount { get; set; }
        public decimal? Temperature { get; set; }
        public string? Notes { get; set; }
    }
}
