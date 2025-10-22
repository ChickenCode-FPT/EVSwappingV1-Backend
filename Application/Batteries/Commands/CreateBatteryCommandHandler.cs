using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Batteries.Commands
{
    public class CreateBatteryCommandHandler : IRequestHandler<CreateBatteryCommand, int>
    {
        private readonly IBatteryRepository _repo;

        public CreateBatteryCommandHandler(IBatteryRepository repo) => _repo = repo;

        public async Task<int> Handle(CreateBatteryCommand request, CancellationToken cancellationToken)
        {
            var entity = new Battery
            {
                BatteryModelId = request.modelId,
            };

            await _repo.Add(entity);
            return entity.BatteryId;
        }
    }
}
