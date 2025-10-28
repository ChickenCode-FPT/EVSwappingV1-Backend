using Application.Common.Interfaces.Repositories;
using Application.Dtos.Battery;
using AutoMapper;
using MediatR;

namespace Application.Batteries.Queries
{

    public record GetBatteriesQuery : IRequest<List<BatteriesDto>>;

    public class GetBatteriesQueryHandler : IRequestHandler<GetBatteriesQuery, List<BatteriesDto>>
    {
        private readonly IBatteryRepository _repo;
        private readonly IMapper _mapper;

        public GetBatteriesQueryHandler(IBatteryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<BatteriesDto>> Handle(GetBatteriesQuery request, CancellationToken cancellationToken)
        {
            var batteries = await _repo.GetAll();
            return _mapper.Map<List<BatteriesDto>>(batteries);
        }
    }
}
