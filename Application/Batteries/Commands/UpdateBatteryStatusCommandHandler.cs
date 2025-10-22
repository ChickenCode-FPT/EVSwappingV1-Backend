using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Batteries.Commands
{
    public class UpdateBatteryStatusCommandHandler : IRequestHandler<UpdateBatteryStatusCommand>
    {
        private readonly IBatteryService _repo;

        public UpdateBatteryStatusCommandHandler(IBatteryService repo) => _repo = repo;

        public async Task<Unit> Handle(UpdateBatteryStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repo.GetById(request.Id);
            if (entity == null) throw new KeyNotFoundException($"Battery {request.Id} not found");

            entity.Status = request.Status.ToString();
            await _repo.Update(entity);
            return Unit.Value;
        }

        Task IRequestHandler<UpdateBatteryStatusCommand>.Handle(UpdateBatteryStatusCommand request, CancellationToken cancellationToken)
        {
            return Handle(request, cancellationToken);
        }
    }
}
