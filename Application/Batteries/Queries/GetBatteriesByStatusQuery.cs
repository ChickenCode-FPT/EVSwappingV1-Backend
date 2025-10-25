using Application.Common.Interfaces.Repositories;
using Domain.Models;
using MediatR;

namespace Application.Batteries.Queries
{
    public record GetBatteriesByStatusQuery(string Status) : IRequest<List<Battery>>;

    public class GetBatteriesByStatusQueryHandler : IRequestHandler<GetBatteriesByStatusQuery, List<Battery>>
    {
        private readonly IBatteryRepository _repo;

        public GetBatteriesByStatusQueryHandler(IBatteryRepository repo) => _repo = repo;

        public async Task<List<Battery>> Handle(GetBatteriesByStatusQuery request, CancellationToken cancellationToken)
        {
            //return await _repo.GetByStatus(request.Status);
            return null;
        } 
    }
}
