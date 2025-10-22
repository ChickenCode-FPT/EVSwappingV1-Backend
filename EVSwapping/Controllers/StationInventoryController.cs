using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [Route("api/stationInventory")]
    [ApiController]
    public class StationInventoryController : ControllerBase
    {
        //private readonly IMediator _mediator;
        //public StationInventoryController(IMediator mediator) => _mediator = mediator;

        //[HttpGet("{stationId}")]
        //public async Task<IActionResult> GetInventory(int stationId)
        //{
        //    var result = await _mediator.Send(new GetStationInventoryQuery(stationId));
        //    return Ok(result);
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetInventories()
        //{
        //    var result = await _mediator.Send(new GetAllStationInventoriesQuery());
        //    return Ok(result);
        //}
    }
}
