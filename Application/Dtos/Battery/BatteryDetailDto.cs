namespace Application.Dtos.Battery
{
    public class BatteryDetailDto
    {
        public int BatteryId { get; set; }
        public string Model { get; set; } = string.Empty;
        public decimal Capacity { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
