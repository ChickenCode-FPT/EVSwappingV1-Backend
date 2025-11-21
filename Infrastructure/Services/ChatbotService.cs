using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Common.Interfaces.Services;
using Application.Dtos;
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
        private const string GetRevenueStatisticsFuncName = "get_revenue_statistics";
        private const string StartDateArg = "startDate";
        private const string EndDateArg = "endDate";
        private const string GroupingArg = "grouping";

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

        public async Task<string> SendMessageAsync(List<ChatHistoryItemDto> chatHistory)
        {
            string promp =
$"""
Bạn là **EV Bot**, trợ lý ảo hỗ trợ quản lý **trạm đổi pin xe điện (EV Battery Swap Station)**.  
Nhiệm vụ của bạn là **giúp người quản lý trạm ra quyết định dựa trên dữ liệu hoạt động**, ví dụ như số lượt đổi pin, giờ cao điểm, công suất sử dụng, v.v.

### Hướng dẫn hành vi:
- Khi người dùng hỏi về **thống kê, báo cáo, hoặc số liệu hoạt động**, hãy **gọi hàm `get_swap_statistics`** để lấy dữ liệu thực tế.
- Nếu người dùng **không nêu rõ khoảng thời gian**, hãy mặc định:
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

### **Định dạng câu trả lời:**
- Câu trả lời phải **ngắn gọn, súc tích, ưu tiên số liệu chính trước**.
- Trình bày dạng **bullet ngắn**, không văn dài dòng.
- Nếu có nhận xét, chỉ thêm **1–2 ý ngắn**.
- Không vượt quá **120 từ**, trừ khi người dùng yêu cầu chi tiết.
- Mẫu định dạng ưu tiên:
  - **Tóm tắt số liệu chính** (1–2 câu ngắn)
  - **Các chỉ số dạng bullet**
  - **Gợi ý hành động ngắn (nếu phù hợp)**

### Thông tin hệ thống:
- Ngày hiện tại (UTC): {DateTime.UtcNow:yyyy-MM-dd}
""";

            // ### Câu hỏi của người dùng:
            // "{chatHistory[0].Text}"

            var history = new List<Content>
            {
                new Content
                {
                    Role = "user",
                    Parts = [new Part { Text = promp }]
                }
            };

            history.AddRange(chatHistory.Select(item => new Content
            {
                Role = item.Role,
                Parts = [new Part { Text = item.Text }]
            }).ToList()
            );

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
            else if (functionCall.Name == GetRevenueStatisticsFuncName)
            {
                var args = functionCall.Args;
                DateTime startDate = DateTime.MinValue, endDate = DateTime.MaxValue;
                string grouping = "month";
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
                    if (args.TryGetValue(GroupingArg, out var groupingVal) && groupingVal is JsonElement groupingElem && groupingElem.ValueKind == JsonValueKind.String)
                    {
                        grouping = groupingVal.ToString()!;
                    }
                }

                Dictionary<DateTime, decimal> revenues = new();
                if (grouping.Equals("day"))
                {
                    revenues = await _statisticService.GetRevenuePerDayAsync(startDate, endDate);
                }
                else
                {
                    revenues = await _statisticService.GetRevenuePerMonthAsync(startDate, endDate);
                }

                functionResultContent = new { revenues };

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
                    },
                new FunctionDeclaration
                {
                    Name = GetRevenueStatisticsFuncName,
                    Description = "Get battery revenue statistics for a given period to forecast demand",
                    Parameters = new Parameters
                    {
                        Type = "OBJECT",
                        Properties = new
                        {
                            startDate = new { type = "STRING", description = "Start date in YYYY-MM-DD format" },
                            endDate = new { type = "STRING", description = "End date in YYYY-MM-DD format" },
                            grouping = new {type = "STRING", description = "Time period to group data. Format: month/day. Default to month"}
                        },
                        Required = new[] { StartDateArg, EndDateArg}
                    }
                },
                }
            };
        }
    }
}
