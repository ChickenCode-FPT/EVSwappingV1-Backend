using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Common.Interfaces.Services;
using Infrastructure.Services.Gemini;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly IStatisticService _statisticService;
        private readonly string _geminiUrl;
        private readonly string _geminiApiKey;

        private const string GetSwapStatisticsFuncName = "get_swap_statistics";
        private const string StartDateArg = "startDate";
        private const string EndDateArg = "endDate";

        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public ChatbotService(HttpClient httpClient, IConfiguration configuration, IStatisticService statisticService)
        {
            _httpClient = httpClient;
            _statisticService = statisticService;
            _geminiUrl = configuration["Gemini:Url"] ?? throw new ArgumentNullException(nameof(configuration), "Gemini Url is not configured.");
            _geminiApiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "Gemini ApiKey is not configured.");
        }

        public async Task<string> SendMessageAsync(string message)
        {
            string promp =
$"""
Bạn là **EV Bot**, trợ lý ảo hỗ trợ quản lý **trạm đổi pin xe điện (EV Battery Swap Station)**.  
Nhiệm vụ của bạn là **giúp người quản lý trạm ra quyết định dựa trên dữ liệu hoạt động**, ví dụ như số lượt đổi pin, giờ cao điểm, công suất sử dụng, v.v.

### Hướng dẫn hành vi:
- Khi người dùng hỏi về **thống kê, báo cáo, hoặc số liệu hoạt động**, hãy **gọi hàm `get_swap_statistics`** để lấy dữ liệu thực tế.
- Nếu người dùng **không nêu rõ khoảng thời gian**, hãy giả định:
  - **Ngày bắt đầu** = 7 ngày trước hôm nay.
  - **Ngày kết thúc** = hôm nay.
- Nếu người dùng nêu “gần đây”, “tuần này”, “tháng này”, v.v., hãy tự suy luận tương ứng:
  - “hôm nay” → ngày hiện tại.
  - “hôm qua” → 1 ngày trước.
  - “tuần này” → từ thứ Hai tuần hiện tại đến hôm nay.
  - “tháng này” → từ ngày 1 của tháng hiện tại đến hôm nay.
- Chỉ sử dụng dữ liệu thực tế trả về từ hàm — **không bịa ra số liệu**.
- Nếu câu hỏi **không liên quan đến dữ liệu hoạt động**, hãy trả lời bằng lời khuyên, giải thích hoặc hướng dẫn hợp lý.
- Luôn trả lời **bằng tiếng Việt**, giọng tự nhiên, chuyên nghiệp, ngắn gọn, dễ hiểu.
- Khi trình bày kết quả, có thể thêm nhận xét hoặc gợi ý hành động (ví dụ: "Giờ cao điểm là 9–11 giờ sáng, nên tăng số lượng pin dự trữ trong khung giờ này.")
- Không được nói về các khái niệm kỹ thuật như “API”, “hàm”, hay “tham số nội bộ”.

### Thông tin hệ thống:
- Ngày hiện tại (UTC): {DateTime.UtcNow:yyyy-MM-dd}

### Câu hỏi của người dùng:
"{message}"
""";
            var history = new List<Content>
            {
                new() { Role = "user", Parts = new[] { new Part { Text = promp } } }
            };

            var request = new GeminiRequest
            {
                Contents = history.ToArray(),
                Tools = new[] { SystemTool() }
            };

            var response = await SendGeminiRequestAsync(request);

            if (response?.GetFunctionCall() != null)
            {
                return await HandleFunctionCallAsync(response, history);
            }

            return response?.GetText() ?? string.Empty;
        }

        private async Task<string> HandleFunctionCallAsync(GeminiResponse response, List<Content> history)
        {
            var functionCall = response.GetFunctionCall()!;
            var geminiContent = response.GetContent()!;

            object functionResultContent;

            if (functionCall.Name == GetSwapStatisticsFuncName)
            {
                var args = functionCall.Args;
                DateTime startDate = DateTime.MinValue, endDate = DateTime.MaxValue;
                if (args != null)
                {
                    if (args.TryGetValue(StartDateArg, out var startVal) && startVal is JsonElement startElem && startElem.ValueKind == JsonValueKind.String)
                    {
                        DateTime.TryParse(startElem.GetString(), out startDate);
                    }
                    if (args.TryGetValue(EndDateArg, out var endVal) && endVal is JsonElement endElem && endElem.ValueKind == JsonValueKind.String)
                    {
                        DateTime.TryParse(endElem.GetString(), out endDate);
                    }
                }

                var swapCount = await _statisticService.GetSwapCountAsync(startDate, endDate);
                var peakHours = await _statisticService.GetPeakHoursAsync(startDate, endDate);
                functionResultContent = new { swapCount, peakHours };
            }
            else
            {
                return "Function not implemented.";
            }

            var toolResponseContent = new Content
            {
                Role = "user",
                Parts = new[]
                {
                    new Part
                    {
                        FunctionResponse = new FunctionResponse
                        {
                            Name = functionCall.Name,
                            Response = functionResultContent
                        }
                    }
                }
            };

            var newHistory = new List<Content>(history) { geminiContent, toolResponseContent };
            var newRequest = new GeminiRequest { Contents = newHistory.ToArray(), Tools = new[] { SystemTool() } };

            var finalResponse = await SendGeminiRequestAsync(newRequest);
            return finalResponse?.GetText() ?? string.Empty;
        }

        private async Task<GeminiResponse?> SendGeminiRequestAsync(GeminiRequest request)
        {
            var jsonRequest = JsonSerializer.Serialize(request, JsonSerializerOptions);
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, _geminiUrl);
            requestMessage.Content = requestContent;
            requestMessage.Headers.Add("X-goog-api-key", _geminiApiKey);

            var httpResponse = await _httpClient.SendAsync(requestMessage);
            httpResponse.EnsureSuccessStatusCode();

            var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<GeminiResponse>(responseStream);
        }

        private static Tool SystemTool()
        {
            return new Tool
            {
                FunctionDeclarations = new[]
                {
                    new FunctionDeclaration
                    {
                        Name = GetSwapStatisticsFuncName,
                        Description = "Get battery swap statistics for a given period to forecast demand",
                        Parameters = new Parameters
                        {
                            Type = "OBJECT",
                            Properties = new
                            {
                                startDate = new { type = "STRING", description = "Start date in YYYY-MM-DD format" },
                                endDate = new { type = "STRING", description = "End date in YYYY-MM-DD format" }
                            },
                            Required = new[] { StartDateArg, EndDateArg }
                        }
                    }
                }
            };
        }
    }
}
