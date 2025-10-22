namespace Application.Dtos
{
    public class VehicleDto
    {
        public int VehicleId { get; set; }
        public string UserId { get; set; }
        public string Vin { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public int? BatteryModelPreferenceId { get; set; }
    }
}
