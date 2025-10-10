namespace Application.Dtos
{
    public class ReservationAllocationDto
    {
        public int ReservationAllocationId { get; set; }
        public int ReservationId { get; set; }
        public int BatteryId { get; set; }
        public DateTime AllocatedAt { get; set; }
        public DateTime HoldUntil { get; set; }
        public string Status { get; set; } = string.Empty;
        public BatteryDto? Battery { get; set; }
    }
}
