using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using AutoMapper;

namespace Application.Services
{
    public class StationService : IStationService
    {
        private readonly IStationRepository _stationRepo;
        private readonly IStationInventoryRepository _inventoryRepo;
        private readonly IMapper _mapper;

        public StationService(IStationRepository stationRepo, IStationInventoryRepository inventoryRepo, IMapper mapper)
        {
            _stationRepo = stationRepo;
            _inventoryRepo = inventoryRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StationDto>> GetNearbyStations(decimal latitude, decimal longitude, double radiusKm)
        {
            var stations = await _stationRepo.GetNearbyStations(latitude, longitude, radiusKm);
            var dtos = _mapper.Map<IEnumerable<StationDto>>(stations);

            foreach (var dto in dtos)
            {
                var available = await _inventoryRepo.GetAvailableBatteries(dto.StationId);
                dto.AvailableBatteries = available.Count();
            }

            return dtos;
        }

        public async Task<StationDto?> GetById(int stationId)
        {
            var station = await _stationRepo.GetById(stationId);
            return _mapper.Map<StationDto>(station);
        }
    }
}
