using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Models;
using MediatR;

namespace Application.Batteries.Commands
{
    public class CreateBatteryCommandHandler : IRequestHandler<CreateBatteryCommand, int>
    {
        private readonly IBatteryRepository _repo;
        private readonly IStationInventoryRepository _inventoryRepository;
        private readonly IInterStationTransferService _interStationTransfer;
        private readonly IMapper _mapper;

        public CreateBatteryCommandHandler(IBatteryRepository repo, IMapper mapper, IStationInventoryRepository inventoryRepository, IInterStationTransferService interStationTransfer)
        {
            _repo = repo;
            _mapper = mapper;
            _inventoryRepository = inventoryRepository;
            _interStationTransfer = interStationTransfer;
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

            var availableSlots = await _interStationTransfer.GetAvaiableSlot(request.StationId);


            if (availableSlots == null || !availableSlots.Any())
            {
                availableSlots = await _inventoryRepository.GetEmptySlots(request.StationId);
            }

            var selectedSlot = availableSlots.First();

            await _inventoryRepository.Add(new StationInventory
            {
                StationId = request.StationId,
                Status = "Empty",
                BatteryId = batteryId,
                SlotNumber = selectedSlot
            });

            return entity.BatteryId;
        }
    }
}
