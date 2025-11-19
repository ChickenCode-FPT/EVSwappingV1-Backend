namespace Application.Dtos.Osrm
{
    // 1 leg là 1 đoạn của tổng tuyến đường
    // A -> B -> C
    public class OsrmLeg
    {
        public double distance { get; set; }
        public double duration { get; set; }
        // tên dg?
        public string summary { get; set; }
        public List<object> steps { get; set; } // hướng dẩn dg đi chi tiết turn-by-turn
    }
}
