using Application.Common.Interfaces;
using Domain.Dtos;
using MediatR;

namespace Application.SubscriptionPackages.Queries
{
    public class GetAllPackagesHandler : IRequestHandler<GetAllPackagesQuery, List<SubscriptionPackageDto>>
    {
        private readonly ISubscriptionPackageService _packageService;

        public GetAllPackagesHandler(ISubscriptionPackageService packageService)
        {
            _packageService = packageService;
        }

        public async Task<List<SubscriptionPackageDto>> Handle(GetAllPackagesQuery request, CancellationToken cancellationToken)
        {
            return await _packageService.GetAllAsync();
        }
    }
}
