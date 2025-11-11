namespace Application.Dtos
{
    public class UpdateBatteryHealthLogDto
    {
        public DateTime RecordedAt { get; set; }
        public decimal SoH { get; set; }
        public int CycleCount { get; set; }
        public decimal Temperature { get; set; }
        public string? Notes { get; set; }
    }
}
