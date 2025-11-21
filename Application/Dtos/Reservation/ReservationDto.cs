using Application.Dtos.Battery;
using Application.Dtos.Station;

namespace Application.Dtos.Reservation
{
    public class ReservationDto
    {
        public int ReservationId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int StationId { get; set; }
        public StationDto? Station { get; set; }
        public int? VehicleId { get; set; }
        public DateTime ReservedFrom { get; set; }
        public DateTime ReservedTo { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? ReservedBatteryModelId { get; set; }
        public BatteryModelDto? BatteryModel { get; set; }
        public string? PaymentCheckoutUrl { get; set; }
        public long? PaymentId { get; set; }
        public string? PaymentStatus { get; set; }
        public ReservationAllocationDto? Allocation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}