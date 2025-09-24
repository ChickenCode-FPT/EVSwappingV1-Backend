using Application.Common.Interfaces;
using Domain.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
