using System;
using System.Collections.Generic;

namespace Application.Dtos;

public class SwapStatisticResponse
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalSwaps { get; set; }
    public List<TimeSeriesDataPoint<int>> DataPoints { get; set; } = new();
}
