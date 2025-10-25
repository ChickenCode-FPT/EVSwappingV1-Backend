namespace Application.Dtos.Osrm
{
    public class OsrmLeg
    {
        public double distance { get; set; }
        public double duration { get; set; }
        public string summary { get; set; }
        public List<object> steps { get; set; } // optional, nếu cần chi tiết turn-by-turn
    }
}
