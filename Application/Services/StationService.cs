using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using Application.Dtos.Osrm;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class StationService : IStationService
    {
        private readonly IStationRepository _stationRepo;
        private readonly IStationInventoryRepository _inventoryRepo;
        private readonly IOSRMService _osrmService;
        private readonly IMapper _mapper;
        private readonly ILogger<StationService> _logger;

        public StationService(
            IStationRepository stationRepo,
            IStationInventoryRepository inventoryRepo,
            IOSRMService osrmService,
            IMapper mapper,
            ILogger<StationService> logger)
        {
            _stationRepo = stationRepo;
            _inventoryRepo = inventoryRepo;
            _osrmService = osrmService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<StationDto>> GetAll()
        {
            var stations = await _stationRepo.GetAll();
            var dtos = stations.Select(s => _mapper.Map<StationDto>(s)).ToList();

            foreach (var dto in dtos)
            {
                dto.AvailableBatteries = await _inventoryRepo.CountAvailableBatteries(dto.StationId);
            }

            return dtos;
        }

        public async Task<StationDto?> GetById(int stationId)
        {
            var station = await _stationRepo.GetById(stationId);
            if (station == null) return null;

            var dto = _mapper.Map<StationDto>(station);
            dto.AvailableBatteries = await _inventoryRepo.CountAvailableBatteries(stationId);

            return dto;
        }

        public async Task<StationDto?> GetNearestStation(decimal userLng, decimal userLat, string profile = "car")
        {
            var stations = (await _stationRepo.GetAll()).ToList();
            if (!stations.Any()) return null;

            var valid = stations
                .Where(s => s.Longitude.HasValue && s.Latitude.HasValue && s.Longitude != 0 && s.Latitude != 0)
                .ToList();

            if (!valid.Any())
            {
                _logger.LogDebug("Không có trạm hợp lệ để tính khoảng cách.");
                return null;
            }

            var start = new CoordinateDto(userLng, userLat);
            var coords = valid.Select(s => new CoordinateDto(s.Longitude!.Value, s.Latitude!.Value));

            var table = await _osrmService.GetTable(start, coords, profile);

            var durations = table.durations?.FirstOrDefault() ?? table.distances?.FirstOrDefault();
            if (durations == null) return null;

            var bestIdx = Array.IndexOf(durations, durations.Min());
            var nearest = valid[bestIdx];

            var dto = _mapper.Map<StationDto>(nearest);
            dto.AvailableBatteries = await _inventoryRepo.CountAvailableBatteries(dto.StationId);

            _logger.LogDebug("Nearest station: {Name} (ID={Id}), Distance={Dist}m", dto.Name, dto.StationId, durations[bestIdx]);

            return dto;
        }

        public async Task<OsrmRouteResponse> GetRouteToStation(decimal userLng, decimal userLat, int stationId, string profile = "car")
        {
            var station = await _stationRepo.GetById(stationId);
            if (station == null) return null!;

            var start = new CoordinateDto(userLng, userLat);
            var end = new CoordinateDto(station.Longitude ?? 0, station.Latitude ?? 0);

            return await _osrmService.GetRoute(start, end, profile);
        }

        public async Task<IEnumerable<StationDto>> GetAllWithDistanceAsync(decimal userLng, decimal userLat)
        {
            var stations = (await _stationRepo.GetAll()).ToList();
            if (!stations.Any())
                return [];

            var valid = stations
                .Where(s => s.Longitude.HasValue && s.Latitude.HasValue)
                .ToList();

            if (!valid.Any())
            {
                _logger.LogWarning("Không có trạm hợp lệ để tính OSRM /table");
                return [];
            }

            // Gọi OSRM Table
            var coords = valid.Select(s => (s.Longitude!.Value, s.Latitude!.Value));
            OsrmTableResponse table;
            try
            {
                table = await _osrmService.GetTable(new CoordinateDto(userLng, userLat),
                                                    coords.Select(c => new CoordinateDto(c.Item1, c.Item2)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi OSRM /table");
                throw;
            }

            var distances = table.distances?.FirstOrDefault();
            var durations = table.durations?.FirstOrDefault();

            var result = new List<StationDto>();

            for (int i = 0; i < valid.Count; i++)
            {
                var dto = _mapper.Map<StationDto>(valid[i]);
                dto.AvailableBatteries = await _inventoryRepo.CountAvailableBatteries(dto.StationId);

                if (distances != null && i < distances.Length)
                    dto.DistanceKm = Math.Round(distances[i] / 1000.0, 2);

                if (durations != null && i < durations.Length)
                    dto.DurationMin = Math.Round(durations[i] / 60.0, 1);

                result.Add(dto);
            }

            return result;
        }
    }
}
