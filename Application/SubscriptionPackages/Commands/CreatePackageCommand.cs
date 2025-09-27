using Domain.Dtos;
using MediatR;

namespace Application.SubscriptionPackages.Commands
{
    public class CreatePackageCommand : IRequest<SubscriptionPackageDto>
    {
        public string Name { get; set; }
        public string BillingCycle { get; set; }
        public decimal Price { get; set; }
        public int IncludedSwaps { get; set; }
    }
}
