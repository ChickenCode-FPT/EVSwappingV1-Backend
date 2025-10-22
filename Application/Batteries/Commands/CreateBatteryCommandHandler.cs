using Application.Common.Interfaces.Repositories;
using Domain.Models;
using MediatR;

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
