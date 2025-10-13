namespace Application.Dtos.Osrm
{
    public class CoordinateDto
    {
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }

        public CoordinateDto() { }

        public CoordinateDto(decimal lng, decimal lat)
        {
            Longitude = lng;
            Latitude = lat;
        }

        public override string ToString() => $"{Longitude},{Latitude}";
    }
}
