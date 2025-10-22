using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Batteries.Commands
{
    public class DeleteBatteryCommandHandler : IRequestHandler<DeleteBatteryCommand>
    {
        private readonly IBatteryService _repo;

        public DeleteBatteryCommandHandler(IBatteryService repo) => _repo = repo;

        public async Task<Unit> Handle(DeleteBatteryCommand request, CancellationToken cancellationToken)
        {
            await _repo.Delete(request.Id);
            return Unit.Value;
        }

        Task IRequestHandler<DeleteBatteryCommand>.Handle(DeleteBatteryCommand request, CancellationToken cancellationToken)
        {
            return Handle(request, cancellationToken);
        }
    }
}
