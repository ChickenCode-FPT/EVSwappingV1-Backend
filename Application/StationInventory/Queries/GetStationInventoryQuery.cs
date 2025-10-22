using Application.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.StationInventory.Queries
{
    public record GetStationInventoryQuery(int StationId): IRequest<StationInventoryDto>;

    public record GetAllStationInventoriesQuery() : IRequest<IEnumerable<StationInventoryDto>>;
}
