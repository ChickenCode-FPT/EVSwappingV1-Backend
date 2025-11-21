using System;

namespace Application.Dtos;

public class RevenueSatisticResponse
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<TimeSeriesDataPoint<decimal>> DataPoints { get; set; } = new();
}
