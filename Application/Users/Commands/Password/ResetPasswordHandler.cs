using Application.Common.Interfaces.Services;
using MediatR;

namespace Application.Users.Commands.Password
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IAuthService _authService;

        public ResetPasswordHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            return await _authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
        }
    }
}
