using System;

namespace Application.Dtos;

public class RatingCreateRequest
{
    public long? SwapTransactionId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int? StationId { get; set; }
    public byte Score { get; set; }
    public string Comment { get; set; } = string.Empty;
}
