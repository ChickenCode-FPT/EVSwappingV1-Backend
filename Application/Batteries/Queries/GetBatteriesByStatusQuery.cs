using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Batteries.Queries
{
    public record GetBatteriesByStatusQuery(string Status) : IRequest<List<Battery>>;

    public class GetBatteriesByStatusQueryHandler : IRequestHandler<GetBatteriesByStatusQuery, List<Battery>>
    {
        private readonly IBatteryService _repo;

        public GetBatteriesByStatusQueryHandler(IBatteryService repo) => _repo = repo;

        public async Task<List<Battery>> Handle(GetBatteriesByStatusQuery request, CancellationToken cancellationToken)
            => await _repo.GetByStatus(request.Status);
    }
}
