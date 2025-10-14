namespace Application.Dtos;

public class RatingDto
{
    public long RatingId { get; set; }
    public long? SwapTransactionId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int? StationId { get; set; }
    public byte Score { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
