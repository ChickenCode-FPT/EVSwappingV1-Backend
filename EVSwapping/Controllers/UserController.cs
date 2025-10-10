using Application.Users.Commands.LockUser;
using Application.Users.Queries.UserManagement;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
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
    }
}
