namespace Application.Dtos.Requests
{
    public class CreateVehicleRequest
    {
        public string UserId { get; set; }
        public string Vin { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public int? BatteryModelPreferenceId { get; set; }
    }
}
