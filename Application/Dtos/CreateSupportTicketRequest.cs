using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

public class CreateSupportTicketRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    public int? StationId { get; set; }
    [Required]
    [StringLength(100)]
    public string Subject { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
}