using Domain.Dtos;
using MediatR;

namespace Application.SubscriptionPackages.Queries
{
    public class GetAllPackagesQuery : IRequest<List<SubscriptionPackageDto>> { }
}
