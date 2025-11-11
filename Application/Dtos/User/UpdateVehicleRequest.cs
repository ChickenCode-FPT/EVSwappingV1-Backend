namespace Application.Dtos.User
{
    public class UpdateVehicleRequest
    {
        public int VehicleId { get; set; }
        public string Vin { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int? Year { get; set; }
        public int? BatteryModelPreferenceId { get; set; }
    }
}
