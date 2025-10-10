namespace Application.Dtos.Osrm
{
    public class OsrmTableResponse
    {
        public string code { get; set; }
        public double[][] distances { get; set; }   // km/m
        public double[][] durations { get; set; }   // sec
        public List<OsrmWaypoint> sources { get; set; }
        public List<OsrmWaypoint> destinations { get; set; }
    }
}
