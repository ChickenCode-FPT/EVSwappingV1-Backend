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
        private readonly IMapper _mapper;

        public BatteryService(IStationInventoryRepository inventoryRepo, IMapper mapper)
        {
            _inventoryRepo = inventoryRepo;
            _mapper = mapper;
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
            var inventories = await _inventoryRepo.GetAvailableBatteries(stationId, batteryModelId);
            return _mapper.Map<IEnumerable<BatteryDto>>(inventories.Select(i => i.Battery));
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
    }
}
