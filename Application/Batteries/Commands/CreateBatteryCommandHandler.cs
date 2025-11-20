using Application.Common.Interfaces.Repositories;
using AutoMapper;
using Domain.Models;
using MediatR;

namespace Application.Batteries.Commands
{
    public class CreateBatteryCommandHandler : IRequestHandler<CreateBatteryCommand, int>
    {
        private readonly IBatteryRepository _repo;
        private readonly IStationInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public CreateBatteryCommandHandler(IBatteryRepository repo, IMapper mapper, IStationInventoryRepository inventoryRepository)
        {
            _repo = repo;
            _mapper = mapper;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<int> Handle(CreateBatteryCommand request, CancellationToken cancellationToken)
        {
            var existingBattery = await _repo.GetBySerialNumber(request.SerialNumber);

            if (existingBattery != null)
            {
                throw new Exception($"A battery with serial number {request.SerialNumber} already exists.");
            }

            var entity = _mapper.Map<Battery>(request);

            entity.CreatedAt = DateTime.UtcNow;

            await _repo.Add(entity);

            var batteryId = entity.BatteryId;

            // Kiểm tra nếu BatteryId không được gán
            if (batteryId == 0)
            {
                throw new Exception("BatteryId was not set after insertion. Check your repository Add method.");
            }

            await _inventoryRepository.Add(new StationInventory
            {
                StationId = request.StationId,
                Status = "Empty",
                BatteryId = batteryId,
                SlotNumber = "1"
            });

            return entity.BatteryId;
        }
    }
}
