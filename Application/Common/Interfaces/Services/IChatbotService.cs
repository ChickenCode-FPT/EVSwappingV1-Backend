using System;

namespace Application.Common.Interfaces.Services;

public interface IChatbotService
{
    Task<string> SendMessageAsync(string message);
}
