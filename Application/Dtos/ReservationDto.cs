namespace Application.Dtos
{
    public class ReservationDto
    {
        public int ReservationId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int StationId { get; set; }
        public int? VehicleId { get; set; }
        public DateTime ReservedFrom { get; set; }
        public DateTime ReservedTo { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? ReservedBatteryModelId { get; set; }
        public ReservationAllocationDto? Allocation { get; set; }
    }
}
