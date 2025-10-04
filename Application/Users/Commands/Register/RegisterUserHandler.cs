using Application.Common.Interfaces.Services;
using MediatR;

namespace Application.Users.Commands.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly IAuthService _authService;
        public RegisterUserHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request.Email, request.Password, request.FullName, request.PhoneNumber);
            return result;
        }
    }
}
