using Application.Common.Interfaces;
using Application.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.StationInventory.Queries
{
    public class GetStationInventoryHandler : IRequestHandler<GetStationInventoryQuery, StationInventoryDto>
    {
        private readonly IStationInventoryService _repo;
        public GetStationInventoryHandler(IStationInventoryService repo) => _repo = repo;

        public async Task<StationInventoryDto> Handle(GetStationInventoryQuery request, CancellationToken ct)
        {
            var station = await _repo.GetInventory(request.StationId, ct);
            return station;
        }
    }
}
