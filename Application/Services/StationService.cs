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

        public async Task<StationDto?> GetNearestStation(decimal userLng, decimal userLat)
        {
            var stations = (await _stationRepo.GetAll()).ToList();
            if (!stations.Any()) return null;

            var valid = stations
                .Select((s, idx) => new { s, idx })
                .Where(x =>
                    x.s.Longitude.HasValue && x.s.Latitude.HasValue &&
                    x.s.Longitude.Value != 0m && x.s.Latitude.Value != 0m)
                .ToList();

            if (!valid.Any())
            {
                _logger.LogDebug("Không có trạm hợp lệ để tính khoảng cách.");
                return null;
            }

            var coords = valid.Select(x => (x.s.Longitude!.Value, x.s.Latitude!.Value));
            var table = await _osrmService.GetTable((userLng, userLat), coords);

            var durations = table.durations?.FirstOrDefault();
            var distances = table.distances?.FirstOrDefault();

            if (durations == null || durations.Length != valid.Count)
            {
                _logger.LogDebug("OSRM trả về kích thước durations không hợp lệ, fallback sang distance.");
                durations = distances;
            }

            if (durations == null)
                throw new Exception("OSRM /table không có dữ liệu duration/distance.");

            var best = durations
                .Select((v, i) => new { v, i })
                .Where(x => !double.IsNaN(x.v) && !double.IsInfinity(x.v))
                .OrderBy(x => x.v)
                .FirstOrDefault();

            if (best == null)
            {
                _logger.LogDebug("Không tìm thấy giá trị hợp lệ trong bảng OSRM.");
                return null;
            }

            var nearest = valid[best.i].s;
            var dto = _mapper.Map<StationDto>(nearest);
            dto.AvailableBatteries = await _inventoryRepo.CountAvailableBatteries(dto.StationId);

            _logger.LogDebug("Nearest station: {Name} (ID={Id}), Index={Idx}, Distance={Dist}m",
                dto.Name, dto.StationId, best.i, best.v);

            return dto;
        }

        public async Task<OsrmRouteResponse> GetRouteToStation(decimal userLng, decimal userLat, int stationId)
        {
            var station = await _stationRepo.GetById(stationId);
            if (station == null) return null!;

            var route = await _osrmService.GetRoute(
                (userLng, userLat),
                (station.Longitude ?? 0m, station.Latitude ?? 0m)
            );

            return route;
        }
    }
}
