using Application.Common.Interfaces.Services;
using MediatR;

namespace Application.Users.Commands.Password
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Unit>
    {
        private readonly IAuthService _authService;

        public ForgotPasswordHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            await _authService.GeneratePasswordResetTokenAsync(request.Email);

            return Unit.Value;
        }
    }
}
