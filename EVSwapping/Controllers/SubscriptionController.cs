using Application.Common.Interfaces;
using Application.Subscriptions.Commands;
using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(IMediator mediator, ISubscriptionService subscriptionService)
        {
            _mediator = mediator;
            _subscriptionService = subscriptionService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterSubscriptionCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("packages")]
        public async Task<IActionResult> GetPackages()
        {
            var packages = await _subscriptionService.GetAllPackagesAsync();
            return Ok(packages);
        }
    }
}
