namespace Application.Dtos.Requests
{
    public class UpdateVehicleRequest
    {
        public int VehicleId { get; set; }
        public string Vin { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public int? BatteryModelPreferenceId { get; set; }
    }
}
