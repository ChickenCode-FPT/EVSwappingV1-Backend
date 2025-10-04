using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using AutoMapper;

namespace Application.Services
{
    public class BatteryService : IBatteryService
    {
        private readonly IStationInventoryRepository _inventoryRepo;
        private readonly IMapper _mapper;

        public BatteryService(IStationInventoryRepository inventoryRepo, IMapper mapper)
        {
            _inventoryRepo = inventoryRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BatteryDto>> GetAvailableBatteries(int stationId, int? batteryModelId = null)
        {
            var inventories = await _inventoryRepo.GetAvailableBatteries(stationId, batteryModelId);
            return _mapper.Map<IEnumerable<BatteryDto>>(inventories.Select(i => i.Battery));
        }
    }
}
