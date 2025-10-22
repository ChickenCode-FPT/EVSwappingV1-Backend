using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Batteries.Commands
{
    public record UpdateBatteryCommand : IRequest<int>
    {
        public int BatteryId { get; set; }
        public string SerialNumber { get; set; }
        public int BatteryModelId { get; set; }
        public decimal? CurrentSoH { get; set; }
        public int? CycleCount { get; set; }
        public string Status { get; set; }
        public DateTime? LastMaintenance { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class UpdateBatteryCommandHandler : IRequestHandler<UpdateBatteryCommand, int>
    {
        private readonly IBatteryRepository _repo;
        private readonly IMapper _mapper;

        public UpdateBatteryCommandHandler(IBatteryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<int> Handle(UpdateBatteryCommand request, CancellationToken cancellationToken)
        {
            var existingBattery = await _repo.GetById(request.BatteryId);

            if (existingBattery == null)
            {
                throw new Exception("Battery not found.");
            }

            _mapper.Map(request, existingBattery);

            await _repo.Update(existingBattery);

            return existingBattery.BatteryId;
        }
    }
}
