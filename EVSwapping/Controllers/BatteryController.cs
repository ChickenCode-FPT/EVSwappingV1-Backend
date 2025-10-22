using Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BatteryController : ControllerBase
    {
        private readonly IBatteryService _batteryService;

        public BatteryController(IBatteryService batteryService)
        {
            _batteryService = batteryService;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableBatteries([FromQuery] int stationId, [FromQuery] int? batteryModelId = null)
        {
            var batteries = await _batteryService.GetAvailableBatteries(stationId, batteryModelId);
            return Ok(batteries);
        }
    }
}
