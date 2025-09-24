using Domain.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.UserManagement
{
    public class GetAllUserCommand : IRequest<List<UserDto>>
    {

    }
}
