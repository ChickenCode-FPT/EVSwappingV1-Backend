namespace Application.Dtos
{
    public class CreateReservationRequest
    {
        public string UserId { get; set; }
        public int StationId { get; set; }
        public int? VehicleId { get; set; }
        public DateTime ReservedFrom { get; set; }
        public DateTime ReservedTo { get; set; }
        public int ReservedBatteryModelId { get; set; }
    }
}
