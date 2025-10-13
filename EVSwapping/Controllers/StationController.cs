using Application.Common.Interfaces.Services;
using Application.Dtos;
using Application.Dtos.Osrm;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stations = await _stationService.GetAll();
            return Ok(stations);
        }

        [HttpGet("{stationId}")]
        public async Task<IActionResult> GetById(int stationId)
        {
            var station = await _stationService.GetById(stationId);
            if (station == null) return NotFound();
            return Ok(station);
        }

        [HttpGet("nearest")]
        public async Task<IActionResult> GetNearest(decimal lng, decimal lat, string profile = "car")
        {
            var nearest = await _stationService.GetNearestStation(lng, lat, profile);
            if (nearest == null) return NotFound();
            return Ok(nearest);
        }

        [HttpGet("{stationId}/route")]
        public async Task<IActionResult> GetRouteToStation(int stationId, decimal lng, decimal lat, string profile = "car")
        {
            var route = await _stationService.GetRouteToStation(lng, lat, stationId, profile);
            if (route == null) return NotFound();
            return Ok(route);
        }

        [HttpGet("with-distance")]
        public async Task<IActionResult> GetAllWithDistance(decimal lng, decimal lat)
        {
            if (lng == 0 || lat == 0)
                return BadRequest("Thiếu toạ độ hợp lệ.");

            var stations = await _stationService.GetAllWithDistanceAsync(lng, lat);
            return Ok(stations);
        }
    }
}
