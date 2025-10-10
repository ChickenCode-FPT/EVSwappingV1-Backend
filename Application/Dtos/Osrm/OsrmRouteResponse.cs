namespace Application.Dtos.Osrm
{
    public class OsrmRouteResponse
    {
        public string code { get; set; }
        public List<OsrmRoute> routes { get; set; }
        public List<OsrmWaypoint> waypoints { get; set; }
    }
}
