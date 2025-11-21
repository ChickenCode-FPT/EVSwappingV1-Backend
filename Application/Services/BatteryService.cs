using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos.Battery;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class BatteryService : IBatteryService
    {
        private readonly IStationInventoryRepository _inventoryRepo;
        private readonly IBatteryRepository _batteryRepo;
        private readonly IMapper _mapper;

        public BatteryService(IStationInventoryRepository inventoryRepo, IBatteryRepository batteryRepo,IMapper mapper)
        {
            _inventoryRepo = inventoryRepo;
            _batteryRepo = batteryRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BatteryDto>> GetIncomingForUser(string userId)
        {
            var list = await _batteryRepo.GetInUseByUser(userId);
            return _mapper.Map<IEnumerable<BatteryDto>>(list);
        }

        public Task Add(Battery battery)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Battery>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BatteryDto>> GetAvailableBatteries(int stationId, int? batteryModelId = null)
        {
            var invs = await _inventoryRepo.GetAvailableBatteries(stationId, batteryModelId);
            return _mapper.Map<IEnumerable<BatteryDto>>(invs.Select(i => i.Battery));
        }

        public Task<Battery?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Battery>> GetByStatus(string status)
        {
            throw new NotImplementedException();
        }

        public Task Update(Battery battery)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BatteryDto>> GetAvailableOutgoing(int stationId, int? batteryModelId)
        {
            var list = await _inventoryRepo.GetAvailableOutgoingBatteries(stationId, batteryModelId);
            return _mapper.Map<IEnumerable<BatteryDto>>(list.Select(i => i.Battery));
        }
    }
}
