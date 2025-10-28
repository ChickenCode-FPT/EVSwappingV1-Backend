using Application.Common.Interfaces.Repositories;
using Application.Dtos.Swap;
using AutoMapper;
using MediatR;

namespace Application.SwapTransactions.Queries
{

    public record GetFullSwapTransactionQuery() : IRequest<List<SwapTranscationFullDto>> { }

    public class GetFullSwapTransactionQueryHandler : IRequestHandler<GetFullSwapTransactionQuery, List<SwapTranscationFullDto>>
    {
        private readonly ISwapTransactionRepository _repo;
        private readonly IMapper _mapper;
        public GetFullSwapTransactionQueryHandler(ISwapTransactionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<SwapTranscationFullDto>> Handle(GetFullSwapTransactionQuery request, CancellationToken cancellationToken)
        {
            var swapTransactions = await _repo.GetAllWithStationAndReversation();
            return _mapper.Map<List<SwapTranscationFullDto>>(swapTransactions);
        }
    }
}
