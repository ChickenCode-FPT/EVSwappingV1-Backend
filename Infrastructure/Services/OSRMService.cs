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

        public OSRMService(HttpClient httpClient, ILogger<OSRMService> logger)
        {
            _http = httpClient;
            _logger = logger;

            if (_http.BaseAddress == null)
            {
                _http.BaseAddress = new Uri("http://127.0.0.1:5000"); // OSRM local server
            }
        }

        // Lấy matrix khoảng cách + thời gian từ user tới nhiều stations
        public async Task<OsrmTableResponse> GetTable((decimal lng, decimal lat) start, IEnumerable<(decimal lng, decimal lat)> stations)
        {
            var stationList = stations.ToList();
            if (!stationList.Any())
                throw new ArgumentException("Không có station hợp lệ để tính OSRM table.");

            var coords = string.Join(";", new[] { $"{start.lng},{start.lat}" }
                .Concat(stationList.Select(s => $"{s.lng},{s.lat}")));

            // sources=0 (user), destinations=1..n (các trạm)
            var destIdx = string.Join(";", Enumerable.Range(1, stationList.Count));
            var url = $"/table/v1/driving/{coords}?annotations=distance,duration&sources=0&destinations={destIdx}";

            _logger.LogDebug("OSRM Table API gọi: {Url}", url);

            var response = await _http.GetFromJsonAsync<OsrmTableResponse>(url);

            if (response == null)
                throw new Exception("OSRM /table API trả về null.");

            if (response.code != "Ok")
                throw new Exception($"OSRM /table API lỗi: {response.code}");

            _logger.LogDebug("OSRM table trả về {Count} điểm hợp lệ.", stationList.Count);

            if (response.distances != null && response.distances.Any())
                _logger.LogDebug("Distances row: {Row}", string.Join(", ", response.distances.First()));

            return response;
        }

        // lấy route chi tiết giữa 2 điểm (polyline, distance, duration)
        public async Task<OsrmRouteResponse> GetRoute((decimal lng, decimal lat) start, (decimal lng, decimal lat) end)
        {
            var url = $"/route/v1/driving/{start.lng},{start.lat};{end.lng},{end.lat}?overview=full&geometries=polyline";

            var response = await _http.GetFromJsonAsync<OsrmRouteResponse>(url);

            if (response == null || response.code != "Ok")
                throw new Exception("OSRM /route API failed");

            return response;
        }
    }
}
