using System;

namespace Application.Dtos;

public record TimeSeriesDataPoint<T>(string Laybel, T Value);

