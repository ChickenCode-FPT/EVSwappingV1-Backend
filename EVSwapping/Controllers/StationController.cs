using Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationController : ControllerBase
    {
        private readonly IStationService _stationService;

        public StationController(IStationService stationService)
        {
            _stationService = stationService;
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyStations([FromQuery] decimal lat, [FromQuery] decimal lng, [FromQuery] double radiusKm = 5)
        {
            var stations = await _stationService.GetNearbyStations(lat, lng, radiusKm);
            return Ok(stations);
        }

        [HttpGet("{stationId}")]
        public async Task<IActionResult> GetById(int stationId)
        {
            var station = await _stationService.GetById(stationId);
            if (station == null) return NotFound();
            return Ok(station);
        }
    }
}
