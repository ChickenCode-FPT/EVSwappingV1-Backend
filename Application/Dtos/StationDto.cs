namespace Application.Dtos
{
    public class StationDto
    {
        public int StationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Capacity { get; set; }
        public string Phone { get; set; } = string.Empty;
        public byte Status { get; set; }
        public int AvailableBatteries { get; set; }
    }
}
