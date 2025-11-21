using System.Collections.Generic;
using Application.Dtos;

namespace Application.Common.Interfaces.Services;

public interface IChatbotService
{
    Task<string> SendMessageAsync(List<ChatHistoryItemDto> history);
}
