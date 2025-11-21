using System.Collections.Generic;

namespace Application.Dtos;

public class ChatbotRequest
{
    public List<ChatHistoryItemDto> History { get; set; }
}
