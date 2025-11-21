using Application.Common.Interfaces.Services;
using Application.Dtos.Osrm;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Infrastructure.Services
{
    public class OSRMService : IOSRMService
    {
        private readonly HttpClient _http;
        private readonly ILogger<OSRMService> _logger;

        private readonly Dictionary<string, string> _baseUrls = new()
        {
            { "car", "http://127.0.0.1:8080/car" },
            { "motorbike", "http://127.0.0.1:8080/motorbike" },
            { "truck", "http://127.0.0.1:8080/truck" }
        };

        public OSRMService(HttpClient httpClient, ILogger<OSRMService> logger)
        {
            _http = httpClient;
            _logger = logger;
        }

        private string GetBaseUrl(string profile)
        {
            if (!_baseUrls.TryGetValue(profile.ToLower(), out var url))
                throw new ArgumentException($"Profile không hợp lệ: {profile}");

            return url;
        }

        // lấy tt tg, khoảng cách từ 1 trạm đến các trạm
        public async Task<OsrmTableResponse> GetTable(
            CoordinateDto start,
            IEnumerable<CoordinateDto> destinations,
            string profile = "car")
        {
            var baseUrl = GetBaseUrl(profile);

            var destList = destinations.ToList();

            if (!destList.Any())
            {
                throw new ArgumentException("Không có destination hợp lệ để tính OSRM table.");
            }

            var coords = string.Join(";", new[] { start.ToString() }.Concat(destList.Select(s => s.ToString())));
            // lon,lat;lon,lat;lon,lat

            var destIdx = string.Join(";", Enumerable.Range(1, destList.Count));
            // danh sách index của destination: 1,2,3...

            var url = $"{baseUrl}/table/v1/driving/{coords}?annotations=distance,duration&sources=0&destinations={destIdx}";
            // sources=0 -> chọn điểm start làm nguồn
            // destinations = destIdx -> các diem còn lại làm đích
            // annotations=distance,duration -> osrm trả về cả 2

            var response = await _http.GetFromJsonAsync<OsrmTableResponse>(url);

            if (response == null || response.code != "Ok")
            {
                throw new Exception($"OSRM /table ({profile}) lỗi: {response?.code ?? "null"}");
            }

            return response;
        }

        // polyline = đường đi được mã hoá thành chuỗi ký tự
        // trả về khoang cách, tg, polyline
        public async Task<OsrmRouteResponse> GetRoute(
            CoordinateDto start,
            CoordinateDto end,
            string profile = "car")
        {
            var baseUrl = GetBaseUrl(profile);
            var url = $"{baseUrl}/route/v1/driving/{start};{end}?overview=full&geometries=polyline";
            // overview=full -> lấy đg đi chi tiết (nhiều điểm)
            // geometries=polyline -> trả về polyline (để vẽ bản đồ)

            var response = await _http.GetFromJsonAsync<OsrmRouteResponse>(url);

            if (response == null || response.code != "Ok")
            {
                throw new Exception($"OSRM /route ({profile}) lỗi.");
            }

            return response;
        }
    }
}
