using Domain.Dtos;
using MediatR;

namespace Application.Users.Queries.UserManagement
{
    public class GetAllUserCommand : IRequest<List<UserDto>>
    {

    }
}
