using Application.Common.Interfaces;
using Domain.Dtos;
using MediatR;

namespace Application.Drivers.Commands
{
    public class RegisterDriverWithPackageHandler
        : IRequestHandler<RegisterDriverWithPackageCommand, RegisterDriverWithPackageResponse>
    {
        private readonly IDriverService _driverService;

        public RegisterDriverWithPackageHandler(IDriverService driverService)
        {
            _driverService = driverService;
        }

        public async Task<RegisterDriverWithPackageResponse> Handle(RegisterDriverWithPackageCommand request, CancellationToken cancellationToken)
        {
            return await _driverService.RegisterDriverWithPackageAsync(
                request.UserId, request.PreferredPaymentMethod, request.PackageId);
        }
    }
}
