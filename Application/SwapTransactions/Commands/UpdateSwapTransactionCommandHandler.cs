using Application.Common.Interfaces.Repositories;
using AutoMapper;
using Domain.Models;
using MediatR;

namespace Application.SwapTransactions.Commands
{
    public record UpdateSwapTransactionCommand : IRequest<int>
    {
        public long SwapTransactionId { get; init; }
        public string CustomerId { get; init; }
        public string? StaffId { get; init; }
        public decimal Fee { get; init; }
        public int? OutgoingBatteryId { get; set; }
        public int? IncomingBatteryId { get; set; }
        public string? SwapStatus { get; init; }
    }

    public class UpdateSwapTransactionCommandHandler : IRequestHandler<UpdateSwapTransactionCommand, int>
    {
        private readonly ISwapTransactionRepository _repo;
        private readonly IStationInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public UpdateSwapTransactionCommandHandler(ISwapTransactionRepository repo, IMapper mapper, IStationInventoryRepository inventoryRepository)
        {
            _repo = repo;
            _mapper = mapper;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<int> Handle(UpdateSwapTransactionCommand request, CancellationToken cancellationToken)
        {
            var tx = await _repo.GetById(request.SwapTransactionId);

            if (tx == null)
            {
                throw new Exception("Swap transaction not found.");
            }

            _mapper.Map(request, tx);

            await _repo.Update(tx);

            var exsitInventory = await _inventoryRepository.GetBybatteryId(request.IncomingBatteryId!.Value);

            if (exsitInventory != null) {
                exsitInventory.BatteryId = request.IncomingBatteryId!.Value;
                exsitInventory.StationId = tx.StationId;
                exsitInventory.Status = "Empty";
                await _inventoryRepository.Update(exsitInventory);
            }

            return tx.OutgoingBatteryId.Value;
        }
    }
}
