using Application.Common.Interfaces.Services;
using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterSubscriptionRequest request)
        {
            try
            {
                var result = await _subscriptionService.RegisterSubscription(request);
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
            var packages = await _subscriptionService.GetAllPackages();
            return Ok(packages);
        }
    }
}
