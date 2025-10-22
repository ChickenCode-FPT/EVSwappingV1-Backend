using Application.Common.Interfaces.Repositories;
using Application.Dtos;
using AutoMapper;
using MediatR;

namespace Application.StationInventories.Queries
{
    public class GetStationInventoryHandler : IRequestHandler<GetStationInventoryQuery, StationInventoryDto>
    {
        private readonly IStationInventoryRepository _repo;
        private readonly IMapper _mapper;
        public GetStationInventoryHandler(IStationInventoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<StationInventoryDto> Handle(GetStationInventoryQuery request, CancellationToken ct)
        {
            var station = await _repo.GetInventory(request.StationId, ct);
            return _mapper.Map<StationInventoryDto>(station);
        }
    }
}
