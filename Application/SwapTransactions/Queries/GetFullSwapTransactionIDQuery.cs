using Application.Common.Interfaces;
using Application.Dtos;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.SwapTransactions.Queries
{
    public record GetFullSwapTransactionIDQuery(int id) : IRequest<SwapTranscationFullDto> { }

    public class GetFullSwapTransactionIDQueryHandler : IRequestHandler<GetFullSwapTransactionIDQuery, SwapTranscationFullDto>
    {
        private readonly ISwapTransactionService _repo;
        private readonly IMapper _mapper;
        public GetFullSwapTransactionIDQueryHandler(ISwapTransactionService repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<SwapTranscationFullDto> Handle(GetFullSwapTransactionIDQuery request, CancellationToken cancellationToken)
        {
            var swapTransactions = await _repo.GetAllWithStationAndReversationID(request.id);
            return _mapper.Map<SwapTranscationFullDto>(swapTransactions);
        }
    }
}
