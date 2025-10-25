using Application.Common.Interfaces.Repositories;
using Application.Dtos;
using AutoMapper;
using MediatR;

namespace Application.StationInventories.Queries
{
    public class GetAllStationInventoriesHandler
        : IRequestHandler<GetAllStationInventoriesQuery, IEnumerable<StationInventoryDto>>
    {
        private readonly IStationInventoryRepository _service;
        private readonly IMapper _mapper;

        public GetAllStationInventoriesHandler(IStationInventoryRepository service, IMapper mapper)
        {
            _mapper = mapper;
            _service = service;
        }

        public async Task<IEnumerable<StationInventoryDto>> Handle(GetAllStationInventoriesQuery request, CancellationToken ct)
        {
            var res = await _service.GetInventorys(ct);
            return _mapper.Map<IEnumerable<StationInventoryDto>>(res);

        }
    }

}
