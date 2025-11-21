using System;

namespace Application.Dtos;

public record TimeSeriesDataPoint<T>(string Lable, T Value);

