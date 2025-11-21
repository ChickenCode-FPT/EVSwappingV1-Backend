using Application.Common.Interfaces.Services;
using Application.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace EVSwapping.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticController : ControllerBase
{
    private readonly IStatisticService _statisticService;

    public StatisticController(IStatisticService statisticService)
    {
        _statisticService = statisticService;
    }

    [HttpGet("revenue/total")]
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

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueSeries(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string period = "month")
    {
        List<TimeSeriesDataPoint<decimal>> dataPoints = [];
        decimal totalRevenue = 0;

        switch (period)
        {
            case "day":
                var revenuePerDay = await _statisticService.GetRevenuePerDayAsync(startDate, endDate);
                totalRevenue = revenuePerDay.Values.Sum();
                dataPoints = revenuePerDay.Select(kvp => new TimeSeriesDataPoint<decimal>(kvp.Key.ToString("yyyy-MM-dd"), kvp.Value)).ToList();
                break;
            case "month":
                var revenuePerMonth = await _statisticService.GetRevenuePerMonthAsync(startDate, endDate);
                totalRevenue = revenuePerMonth.Values.Sum();
                dataPoints = revenuePerMonth.Select(kvp => new TimeSeriesDataPoint<decimal>(kvp.Key.ToString("yyyy-MM-dd"), kvp.Value)).ToList();
                break;
            case "quarter":
                break;
            case "year":
                break;
            default:
                var revenue = await _statisticService.GetRevenuePerDayAsync(startDate, endDate);
                totalRevenue = revenue.Values.Sum();
                dataPoints = revenue.Select(kvp => new TimeSeriesDataPoint<decimal>(kvp.Key.ToString("yyyy-MM-dd"), kvp.Value)).ToList();
                break;
        }

        var response = new RevenueSatisticResponse
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalRevenue = totalRevenue,
            DataPoints = dataPoints
        };

        return Ok(response);
    }

    [HttpGet("swap/total")]
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

    [HttpGet("swap/peak-hours")]
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

    [HttpGet("swap")]
    public async Task<IActionResult> GetSwapSeries(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string period = "month")
    {
        List<TimeSeriesDataPoint<int>> dataPoints = [];
        int totalSwaps = 0;

        switch (period)
        {
            case "day":
                var swapsPerDay = await _statisticService.GetSwapCountPerDayAsync(startDate, endDate);
                totalSwaps = swapsPerDay.Values.Sum();
                dataPoints = swapsPerDay.Select(kvp => new TimeSeriesDataPoint<int>(kvp.Key.ToString("yyyy-MM-dd"), kvp.Value)).ToList();
                break;
            case "month":
                var swapsPerMonth = await _statisticService.GetSwapCountPerMonthAsync(startDate, endDate);
                totalSwaps = swapsPerMonth.Values.Sum();
                dataPoints = swapsPerMonth.Select(kvp => new TimeSeriesDataPoint<int>(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(kvp.Key - 1), kvp.Value)).ToList();
                break;
            case "quarter":
                var swapsPerQuarter = await _statisticService.GetSwapCountPerQuarterAsync(startDate, endDate);
                totalSwaps = swapsPerQuarter.Values.Sum();
                dataPoints = swapsPerQuarter.Select(kvp => new TimeSeriesDataPoint<int>($"Q{kvp.Key}", kvp.Value)).ToList();
                break;
            case "year":
                var swapsPerYear = await _statisticService.GetSwapCountPerYearAsync(startDate, endDate);
                totalSwaps = swapsPerYear.Values.Sum();
                dataPoints = swapsPerYear.Select(kvp => new TimeSeriesDataPoint<int>(kvp.Key.ToString(), kvp.Value)).ToList();
                break;
            default:
                var swaps = await _statisticService.GetSwapCountPerDayAsync(startDate, endDate);
                totalSwaps = swaps.Values.Sum();
                dataPoints = swaps.Select(kvp => new TimeSeriesDataPoint<int>(kvp.Key.ToString("yyyy-MM-dd"), kvp.Value)).ToList();
                break;
        }

        var response = new SwapStatisticResponse
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalSwaps = totalSwaps,
            DataPoints = dataPoints
        };

        return Ok(response);
    }
}