namespace Application.Dtos.Osrm
{
    public class OsrmWaypoint
    {
        public string hint { get; set; }
        public double[] location { get; set; }  // [lng, lat]
        public string name { get; set; }
    }
}
