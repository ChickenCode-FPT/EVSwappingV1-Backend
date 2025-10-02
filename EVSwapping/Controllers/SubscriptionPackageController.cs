using Application.Common.Interfaces.Services;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionPackageController : ControllerBase
    {
        private readonly ISubscriptionPackageService _packageService;

        public SubscriptionPackageController(ISubscriptionPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPackages()
        {
            var result = await _packageService.GetAll();
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreatePackageRequest request)
        {
            try
            {
                var result = await _packageService.Create(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
