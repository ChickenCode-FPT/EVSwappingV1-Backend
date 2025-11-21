using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos.Battery;
using AutoMapper;
using Domain.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.UserManagement
{
    public record GetUserQuery(string userId) : IRequest<StaffDto>;


    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, StaffDto>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<StaffDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUser(request.userId);
            return _mapper.Map<StaffDto>(user);
        }
    }
}
