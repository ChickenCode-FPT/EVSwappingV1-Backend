using Application.Common.Interfaces;
using Domain.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.Login
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
    {
        private readonly IAuthService _authService;

        public LoginUserHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var token = await _authService.LoginAsync(request.Email, request.Password);
            return token;
        }
    }

}
