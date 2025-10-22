using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Dtos;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Batteries.Queries
{
    public record GetBatteriesIDQuery(int id): IRequest<BatteriesDto> {}

    public class GetBatteriesIDQueryHandler : IRequestHandler<GetBatteriesIDQuery, BatteriesDto> 
    {
        private readonly IBatteryRepository _repo;
        private readonly IMapper _mapper;

        public GetBatteriesIDQueryHandler(IBatteryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<BatteriesDto> Handle(GetBatteriesIDQuery request, 
            CancellationToken cancellationToken)
        {
            var batteries = await _repo.GetById(request.id);
            return _mapper.Map<BatteriesDto>(batteries);
        }
    }
}
