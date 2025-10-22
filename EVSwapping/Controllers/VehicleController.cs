using Application.Common.Interfaces.Services;
using Application.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vehicle = await _vehicleService.GetById(id);
            if (vehicle == null) return NotFound();
            return Ok(vehicle);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var vehicles = await _vehicleService.GetByUser(userId);
            return Ok(vehicles);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVehicleRequest request)
        {
            var vehicle = await _vehicleService.Create(request);
            return Ok(vehicle);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateVehicleRequest request)
        {
            request.VehicleId = id;
            var vehicle = await _vehicleService.Update(request);
            return Ok(vehicle);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _vehicleService.Delete(id);
            return NoContent();
        }
    }
}
