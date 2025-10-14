namespace Application.Dtos;

public class SupportTicketDto
{
    public long TicketId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int? StationId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}