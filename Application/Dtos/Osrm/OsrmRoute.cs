namespace Application.Dtos.Osrm
{
    public class OsrmRoute
    {
        public double distance { get; set; }   // meters
        public double duration { get; set; }   // seconds
        public string geometry { get; set; }   // polyline encoded
        public string weight_name { get; set; }
        public double weight { get; set; }
        public List<OsrmLeg> legs { get; set; }
    }
}
