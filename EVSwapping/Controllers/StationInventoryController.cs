using Application.Batteries.Commands;
using Application.Dtos.Station;
using Application.StationInventories.Commands;
using Application.StationInventories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [Route("api/stationInventory")]
    [ApiController]
    public class StationInventoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public StationInventoryController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{stationId}")]
        public async Task<IActionResult> GetInventory(int stationId)
        {
            var result = await _mediator.Send(new GetStationInventoryQuery(stationId));
            return Ok(result);
        }

        [HttpGet("/api/stationInventories")]
        public async Task<IActionResult> GetInventories()
        {
            var result = await _mediator.Send(new GetAllStationInventoriesQuery());
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateInventory([FromBody] UpdateStatusCommand command)
        {

            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { message = "Inventory successfully.", StationInventoryId = result });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("duplicate"))
                {
                    return BadRequest(new { message = ex.Message });
                }

                return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
            }
        }
    }
}
