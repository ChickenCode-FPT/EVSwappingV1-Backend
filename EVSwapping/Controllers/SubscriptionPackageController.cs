using Application.Common.Interfaces.Services;
using Application.Dtos;
using Application.Dtos.Subscription;
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] UpdatePackageRequest request)
        {
            try
            {
                await _packageService.Update(id, request);
                return Ok(new { message = "Package updated successfully." });
            }
            catch (Exception ex) 
            {
                return BadRequest(new { error = ex.Message });
            }

        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActivePackages()
        {
            try
            {
                var result = await _packageService.GetActivePackages();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("inactive/{id}")]
        public async Task<IActionResult> InactivePackage(int id)
        {
            try
            {
                await _packageService.InactivePackage(id);
                return Ok(new { message = "Package set to inactive successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("reactivate/{id}")]
        public async Task<IActionResult> ReactivatePackage(int id)
        {
            try
            {
                await _packageService.ReactivatePackage(id);
                return Ok(new { message = "Package reactivated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPut("publish/{id}")]
        public async Task<IActionResult> PublishPackage(int id)
        {
            try
            {
                await _packageService.PublishPackage(id);
                return Ok(new { message = "Package published successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
