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
            { "car", "http://127.0.0.1:5000" },
            { "motorbike", "http://127.0.0.1:5001" },
            { "truck", "http://127.0.0.1:5002" }
            // Hoặc dùng proxy:
            // { "car", "http://127.0.0.1:8080/car" },
            // { "motorbike", "http://127.0.0.1:8080/motorbike" },
            // { "truck", "http://127.0.0.1:8080/truck" },
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

        public async Task<OsrmTableResponse> GetTable(
            CoordinateDto start,
            IEnumerable<CoordinateDto> destinations,
            string profile = "car")
        {
            var baseUrl = GetBaseUrl(profile);
            var destList = destinations.ToList();
            if (!destList.Any())
                throw new ArgumentException("Không có destination hợp lệ để tính OSRM table.");

            var coords = string.Join(";", new[] { start.ToString() }
                .Concat(destList.Select(s => s.ToString())));

            var destIdx = string.Join(";", Enumerable.Range(1, destList.Count));
            var url = $"{baseUrl}/table/v1/driving/{coords}?annotations=distance,duration&sources=0&destinations={destIdx}";

            _logger.LogDebug("OSRM Table API ({Profile}) gọi: {Url}", profile, url);

            var response = await _http.GetFromJsonAsync<OsrmTableResponse>(url);

            if (response == null || response.code != "Ok")
                throw new Exception($"OSRM /table ({profile}) lỗi: {response?.code ?? "null"}");

            return response;
        }

        public async Task<OsrmRouteResponse> GetRoute(
            CoordinateDto start,
            CoordinateDto end,
            string profile = "car")
        {
            var baseUrl = GetBaseUrl(profile);
            var url = $"{baseUrl}/route/v1/driving/{start};{end}?overview=full&geometries=polyline";

            _logger.LogDebug("OSRM Route API ({Profile}) gọi: {Url}", profile, url);

            var response = await _http.GetFromJsonAsync<OsrmRouteResponse>(url);

            if (response == null || response.code != "Ok")
                throw new Exception($"OSRM /route ({profile}) lỗi.");

            return response;
        }
    }
}
