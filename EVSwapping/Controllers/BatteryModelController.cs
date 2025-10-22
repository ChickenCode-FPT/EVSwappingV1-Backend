using Application.Common.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [Route("api/batteryModel")]
    [ApiController]
    public class BatteryModelController : ControllerBase
    {
        private readonly IBatteryModelService _batteryModelService;

        public BatteryModelController(IBatteryModelService batteryModelService)
        {
            _batteryModelService = batteryModelService;
        }

        // GET: api/batterymodel
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var batteryModels = await _batteryModelService.GetAll();
                return Ok(batteryModels);
            }
            catch (Exception ex)
            {
                // You can log the exception here
                return StatusCode(500, "An error occurred while retrieving the battery models.");
            }
        }

        // GET: api/batterymodel/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var batteryModel = await _batteryModelService.GetById(id);

                if (batteryModel == null)
                {
                    return NotFound($"Battery model with ID {id} not found.");
                }

                return Ok(batteryModel);
            }
            catch (Exception ex)
            {
                // You can log the exception here
                return StatusCode(500, "An error occurred while retrieving the battery model.");
            }
        }

        // POST: api/batterymodel
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] BatteryModel model)
        {
            if (model == null)
            {
                return BadRequest("Battery model is null.");
            }

            try
            {
                await _batteryModelService.Add(model);
                return CreatedAtAction(nameof(GetById), new { id = model.BatteryModelId }, model);
            }
            catch (Exception ex)
            {
                // You can log the exception here
                return StatusCode(500, "An error occurred while adding the battery model.");
            }
        }

        // PUT: api/batterymodel/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BatteryModel model)
        {
            if (model == null || model.BatteryModelId != id)
            {
                return BadRequest("Battery model is null or ID mismatch.");
            }

            try
            {
                await _batteryModelService.Update(model);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                // You can log the exception here
                return StatusCode(500, "An error occurred while updating the battery model.");
            }
        }

        // DELETE: api/batterymodel/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _batteryModelService.Delete(id);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                // You can log the exception here
                return StatusCode(500, "An error occurred while deleting the battery model.");
            }
        }
    }
}
