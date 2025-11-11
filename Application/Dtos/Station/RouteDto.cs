public class RouteDto
{
    public double DistanceKm { get; set; }
    public double DurationMin { get; set; }
    public string Polyline { get; set; } // encoded polyline từ OSRM
}
