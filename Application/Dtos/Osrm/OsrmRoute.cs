namespace Application.Dtos.Osrm
{
    public class OsrmRoute
    {
        // tổng chiều dài tuyen đường
        public double distance { get; set; }   // meters
        // tổng tg di chuyển
        public double duration { get; set; }   // seconds
        public string geometry { get; set; }   // polyline encoded
        // loại thuật toán osrm sử sụng
        public string weight_name { get; set; }
        // giá trị logic osrm dùng để tối ưu 
        public double weight { get; set; }
        // đoạn dg dữa các waypoint
        public List<OsrmLeg> legs { get; set; }
    }
}
