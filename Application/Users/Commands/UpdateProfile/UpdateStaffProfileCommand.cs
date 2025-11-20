using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Users.Commands.Register;
using AutoMapper;
using Domain.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.UpdateProfile
{
    public class UpdateStaffProfileCommand: IRequest<StaffUpdateDto>
    {
        public string UserId { get; set; } = "";
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string FId { get; set; } = "";
    }

    public class UpdateStaffProfileHandler : IRequestHandler<UpdateStaffProfileCommand, StaffUpdateDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UpdateStaffProfileHandler(IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }
        public async Task<StaffUpdateDto> Handle(UpdateStaffProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUser(request.UserId);


            if (user == null)
            {
                throw new Exception("User not found.");
            }

            user.Email = request.Email;
            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;
            user.FId = request.FId;

            await _userRepository.UpdateUser(user);

            var staffUpdateDto = _mapper.Map<StaffUpdateDto>(user);

            return staffUpdateDto;
        }
    }
}
