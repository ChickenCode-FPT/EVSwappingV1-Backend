using Application.Common.Interfaces.Services;
using Domain.Dtos;
using MediatR;

namespace Application.Users.Queries.UserManagement
{
    public class GetAllUserHandle : IRequestHandler<GetAllUserCommand, List<UserDto>>
    {
        private readonly IUserService _userService;
        public GetAllUserHandle(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<List<UserDto>> Handle(GetAllUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userList = await _userService.GetAllUserDtoAsync();
                return userList;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return new List<UserDto>();
            }
        }
    }
}
