namespace Application.Dtos.Osrm
{
    public class CoordinateDto
    {
        // kinh độ (Đông - Tây)
        public decimal Longitude { get; set; }

        // vĩ độ (Bắc - Nam)
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
