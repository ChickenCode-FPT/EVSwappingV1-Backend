using Application.Dtos;

namespace Application.Common.Interfaces.Services;
public interface ISupportTicketService
{
    Task<SupportTicketDto> CreateTicketAsync(CreateSupportTicketRequest request);
    Task<SupportTicketDto> GetTicketByIdAsync(long ticketId);
    Task<IEnumerable<SupportTicketDto>> GetAllTicketsAsync();
    Task<IEnumerable<SupportTicketDto>> GetTicketsByUserIdAsync(string userId);
    Task<IEnumerable<SupportTicketDto>> GetTicketsByStationIdAsync(int stationId);
    Task<SupportTicketDto> UpdateTicketStatusAsync(long ticketId, string newStatus);
    Task CloseTicketAsync(long ticketId);
}