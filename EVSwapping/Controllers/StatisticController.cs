using Application.Common.Interfaces.Services;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetTotalRevenue([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var revenue = await _statisticService.GetTotalRevenueAsync(startDate, endDate);
            var response = new RevenueSatisticResponse
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = revenue
            };
            return Ok(response);
        }

        [HttpGet("revenue/per-day")]
        public async Task<IActionResult> GetRevenuePerDay([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var revenuePerDay = await _statisticService.GetRevenuePerDayAsync(startDate, endDate);
            var response = new RevenueSatisticResponse
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = revenuePerDay.Values.Sum(),
                DataPoints = revenuePerDay.Select(kvp => new TimeSeriesDataPoint<decimal>(kvp.Key.ToString("yyyy-MM-dd"), kvp.Value)).ToList()
            };
            return Ok(response);
        }

        [HttpGet("revenue/per-month")]
        public async Task<IActionResult> GetRevenuePerMonth([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var revenuePerMonth = await _statisticService.GetRevenuePerMonthAsync(startDate, endDate);
            var response = new RevenueSatisticResponse
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = revenuePerMonth.Values.Sum(),
                DataPoints = revenuePerMonth.Select(kvp => new TimeSeriesDataPoint<decimal>(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(kvp.Key), kvp.Value)).ToList()
            };
            return Ok(response);
        }

        [HttpGet("swaps/count")]
        public async Task<IActionResult> GetSwapCount([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var swapCount = await _statisticService.GetSwapCountAsync(startDate, endDate);
            var response = new SwapStatisticResponse
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalSwaps = swapCount
            };
            return Ok(response);
        }

        [HttpGet("swaps/peak-hours")]
        public async Task<IActionResult> GetPeakHours([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var peakHours = await _statisticService.GetPeakHoursAsync(startDate, endDate);
            var response = new SwapStatisticResponse
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalSwaps = peakHours.Values.Sum(),
                DataPoints = peakHours.Select(kvp => new TimeSeriesDataPoint<int>($"{kvp.Key}:00 - {kvp.Key + 1}:00", kvp.Value)).ToList()
            };
            return Ok(response);
        }
    }
}