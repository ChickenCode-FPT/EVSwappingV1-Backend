using Application.Batteries.Commands;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.StationInventories.Commands
{
    public record UpdateStatusCommand : IRequest<int>
    {
        public int StationInventoryId { get; set; }
        public string Status { get; set; }
    }

    public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand, int>
    {
        private readonly IBatteryRepository _repo;
        private readonly IStationInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public UpdateStatusCommandHandler(IMapper mapper, IStationInventoryRepository inventoryRepository, IBatteryRepository repo)
        {
            _mapper = mapper;
            _inventoryRepository = inventoryRepository;
            _repo = repo;
        }

        public async Task<int> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
        {
            var existingInventory = await _inventoryRepository.GetById(request.StationInventoryId);

            if (existingInventory == null)
            {
                throw new Exception($"Inventory with ID {request.StationInventoryId} not found.");
            }

            existingInventory.Status = request.Status;
            existingInventory.CheckedAt = DateTime.UtcNow;

            await _inventoryRepository.Update(existingInventory);

            if (existingInventory.Battery != null)
            {
                var battery = existingInventory.Battery;
                battery.Status = request.Status;
                battery.LastMaintenance = DateTime.UtcNow;
                await _repo.Update(battery);
            }

            return existingInventory.StationInventoryId;
        }
    }
}
