using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;
public class SupportTicketService : ISupportTicketService
{
    private readonly ISupportTicketRepository _ticketRepo;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IStationRepository _stationRepository;

    public SupportTicketService(ISupportTicketRepository ticketRepo, IMapper mapper, UserManager<User> userManager, IStationRepository stationRepository)
    {
        _ticketRepo = ticketRepo;
        _mapper = mapper;
        _userManager = userManager;
        _stationRepository = stationRepository;
    }

    public async Task CloseTicketAsync(long ticketId)
    {
        var ticket = await _ticketRepo.GetAsync(t => t.TicketId == ticketId);
        if (ticket == null)
        {
            throw new NotFoundException($"Ticket with ID {ticketId} not found.");
        }
        ticket.Status = "Closed";
        await _ticketRepo.UpdateAsync(ticket);
    }

    public async Task<SupportTicketDto> CreateTicketAsync(CreateSupportTicketRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {request.UserId} not found.");
        }

        if (request.StationId.HasValue)
        {
            var station = await _stationRepository.GetById(request.StationId.Value);
            if (station == null)
            {
                throw new NotFoundException($"Station with ID {request.StationId} not found.");
            }
        }

        var ticket = new SupportTicket
        {
            UserId = request.UserId,
            StationId = request.StationId,
            Subject = request.Subject,
            Description = request.Description,
            Status = "Open",
            CreatedAt = DateTime.UtcNow
        };

        var created = await _ticketRepo.AddAsync(ticket);

        return _mapper.Map<SupportTicketDto>(created);
    }

    public async Task<IEnumerable<SupportTicketDto>> GetAllTicketsAsync()
    {
        var tickets = await _ticketRepo.GetAllAsync();

        return _mapper.Map<IEnumerable<SupportTicketDto>>(tickets);
    }

    public async Task<SupportTicketDto> GetTicketByIdAsync(long ticketId)
    {
        var ticket = await _ticketRepo.GetAsync(t => t.TicketId == ticketId);
        if (ticket == null)
        {
            throw new NotFoundException($"Ticket with ID {ticketId} not found.");
        }
        return _mapper.Map<SupportTicketDto>(ticket);
    }

    public async Task<IEnumerable<SupportTicketDto>> GetTicketsByStationIdAsync(int stationId)
    {
        var tickets = await _ticketRepo.GetAllAsync(t => t.StationId == stationId);

        return _mapper.Map<IEnumerable<SupportTicketDto>>(tickets);
    }

    public async Task<IEnumerable<SupportTicketDto>> GetTicketsByUserIdAsync(string userId)
    {
        var tickets = await _ticketRepo.GetAllAsync(t => t.UserId == userId);

        return _mapper.Map<IEnumerable<SupportTicketDto>>(tickets);
    }

    public async Task<SupportTicketDto> UpdateTicketStatusAsync(long ticketId, string newStatus)
    {
        var ticket = await _ticketRepo.GetAsync(t => t.TicketId == ticketId);

        if (ticket == null)
        {
            throw new NotFoundException($"Ticket with ID {ticketId} not found.");
        }

        ticket.Status = newStatus;

        var updated = await _ticketRepo.UpdateAsync(ticket);

        return _mapper.Map<SupportTicketDto>(updated);
    }
}


