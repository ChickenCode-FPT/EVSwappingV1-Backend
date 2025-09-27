using Application.Common.Interfaces;
using Domain.Dtos;
using MediatR;

namespace Application.Drivers.Commands
{
    public class RegisterDriverHandler : IRequestHandler<RegisterDriverCommand, RegisterDriverResponse>
    {
        private readonly IDriverService _driverService;

        public RegisterDriverHandler(IDriverService driverService)
        {
            _driverService = driverService;
        }

        public async Task<RegisterDriverResponse> Handle(RegisterDriverCommand request, CancellationToken cancellationToken)
        {
            return await _driverService.RegisterDriverAsync(
                request.UserId, request.PreferredPaymentMethod);
        }
    }
}
