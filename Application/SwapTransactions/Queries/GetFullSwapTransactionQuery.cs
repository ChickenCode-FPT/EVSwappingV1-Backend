using Application.Common.Interfaces;
using Application.Dtos;
using AutoMapper;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.SwapTransactions.Queries
{

    public record GetFullSwapTransactionQuery(): IRequest<List<SwapTranscationFullDto>> {}

    public class GetFullSwapTransactionQueryHandler : IRequestHandler<GetFullSwapTransactionQuery, List<SwapTranscationFullDto>>
    {
        private readonly ISwapTransactionService _repo;
        private readonly IMapper _mapper;
        public GetFullSwapTransactionQueryHandler(ISwapTransactionService repo, IMapper mapper)
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
