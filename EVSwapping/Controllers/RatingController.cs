using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRating([FromBody] RatingCreateRequest request)
        {
            try
            {
                var rating = await _ratingService.CreateRating(request);
                return Ok(rating);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRatings()
        {
            try
            {
                var ratings = await _ratingService.GetAllRatings();
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> GetStationRatings(int stationId)
        {
            try
            {
                var ratings = await _ratingService.GetStationRatings(stationId);
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRating([FromBody] RatingDto ratingDto)
        {
            try
            {
                var rating = await _ratingService.UpdateRating(ratingDto);
                return Ok(rating);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpDelete("{ratingId}")]
        public async Task<IActionResult> DeleteRating(int ratingId)
        {
            try
            {
                await _ratingService.DeleteRating(ratingId);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}

