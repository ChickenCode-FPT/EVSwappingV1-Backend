using Application.Common.Interfaces;
using Domain.Dtos;
using MediatR;

namespace Application.SubscriptionPackages.Commands
{
    public class CreatePackageHandler : IRequestHandler<CreatePackageCommand, SubscriptionPackageDto>
    {
        private readonly ISubscriptionPackageService _packageService;

        public CreatePackageHandler(ISubscriptionPackageService packageService)
        {
            _packageService = packageService;
        }

        public async Task<SubscriptionPackageDto> Handle(CreatePackageCommand request, CancellationToken cancellationToken)
        {
            return await _packageService.CreateAsync(request);
        }
    }
}
