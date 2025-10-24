using Application.Common.Interfaces.Services;
using Application.Dtos;
using Application.Users.Commands.LockUser;
using Application.Users.Queries.UserManagement;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EVSwapping.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IMediator _mediator;
        public readonly IUserService _userService;

        public UserController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _mediator.Send(new GetAllUserCommand());
            return Ok(users);
        }

        [HttpPost("lock/{id}")]
        public async Task<IActionResult> LockUser(string id)
        {
            var success = await _mediator.Send(new LockUserCommand(id));
            if (!success) return NotFound();
            return Ok();
        }

        [HttpPost("unlock/{id}")]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var success = await _mediator.Send(new UnlockUserCommand(id));
            if (!success) return NotFound();
            return Ok();
        }

        [HttpPut("{userId}/promote")]
        public async Task<IActionResult> PromoteUser(string userId, [FromBody] PromoteUserDto dto)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _userService.PromoteUserRoleAsync(
                userId,
                dto.NewRole,
                adminId!,
                dto.ReplaceExistingRoles
            );

            return Ok(new { message = $"User {userId} được nâng role thành {dto.NewRole}" });
        }
    }
}
