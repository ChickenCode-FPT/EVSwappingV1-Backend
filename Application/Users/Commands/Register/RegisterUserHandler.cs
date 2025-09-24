using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.Register
{
    public class RegisterUserHandler :IRequestHandler<RegisterUserCommand, string>
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
