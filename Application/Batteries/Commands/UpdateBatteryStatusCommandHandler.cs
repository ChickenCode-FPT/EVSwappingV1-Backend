using Application.Common.Interfaces.Repositories;
using Application.StationInventories.Commands;
using AutoMapper;
using MediatR;

namespace Application.Batteries.Commands
{
    public class UpdateBatteryStatusCommandHandler : IRequestHandler<UpdateBatteryStatusCommand, int>
    {
        private readonly IBatteryRepository _repo;
        private readonly IStationInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public UpdateBatteryStatusCommandHandler(IMapper mapper, IStationInventoryRepository inventoryRepository, IBatteryRepository repo)
        {
            _mapper = mapper;
            _inventoryRepository = inventoryRepository;
            _repo = repo;
        }

        public async Task<int> Handle(UpdateBatteryStatusCommand request, CancellationToken cancellationToken)
        {
            var existingBattery = await _repo.GetById(request.BatteryId);

            if (existingBattery == null)
            {
                throw new Exception($"Battery with ID {request.BatteryId} not found.");
            }

            existingBattery.Status = request.Status;
            existingBattery.LastMaintenance = DateTime.UtcNow;

            await _repo.Update(existingBattery);

            var newStatus = request.Status?.Trim().ToLowerInvariant();

            if (request.Status == "full")
            {
                var existingInventory = await _inventoryRepository.GetBybatteryId(request.BatteryId);

                if (existingInventory != null)
                {
                    existingInventory.Status = "Full";

                    await _inventoryRepository.Update(existingInventory);
                }
            }
            return existingBattery.BatteryId;
        }
    }
}
