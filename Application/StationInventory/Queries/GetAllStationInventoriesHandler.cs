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
    public class GetAllStationInventoriesHandler
        : IRequestHandler<GetAllStationInventoriesQuery, IEnumerable<StationInventoryDto>>
    {
        private readonly IStationInventoryService _service;
        public GetAllStationInventoriesHandler(IStationInventoryService service) => _service = service;

        public async Task<IEnumerable<StationInventoryDto>> Handle(GetAllStationInventoriesQuery request, CancellationToken ct)
        {
            return await _service.GetInventories(ct);
        }
    }

}
