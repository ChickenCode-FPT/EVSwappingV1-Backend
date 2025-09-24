using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.Login
{
    public class GoogleLoginCommand : IRequest<GoogleLoginResponseDto>
    {
        public HttpContext HttpContext { get; set; }

        public GoogleLoginCommand(HttpContext httpContext)
        {
            HttpContext = httpContext;
        }
    }
}
