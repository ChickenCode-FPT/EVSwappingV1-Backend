using System.Text.Json.Serialization;

namespace Infrastructure.Services.Gemini
{
    public class GeminiRequest
    {
        [JsonPropertyName("contents")]
        public Content[]? Contents { get; set; }
        [JsonPropertyName("tools")]
        public Tool[]? Tools { get; set; }
    }

    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public Candidate[]? Candidates { get; set; }

        public string? GetText() => Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
        public FunctionCall? GetFunctionCall() => Candidates?.FirstOrDefault()?.Content?.Parts?.LastOrDefault()?.FunctionCall;
        public Content? GetContent() => Candidates?.FirstOrDefault()?.Content;
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content? Content { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public Part[]? Parts { get; set; }
        [JsonPropertyName("role")]
        public string? Role { get; set; }
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
        [JsonPropertyName("functionCall")]
        public FunctionCall? FunctionCall { get; set; }
        [JsonPropertyName("function_response")]
        public FunctionResponse? FunctionResponse { get; set; }
    }

    public class FunctionResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("content")]
        public object? Content { get; set; }
        [JsonPropertyName("response")]
        public object? Response { get; set; }
    }

    public class FunctionCall
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("args")]
        public Dictionary<string, object>? Args { get; set; }
    }

    public class Tool
    {
        [JsonPropertyName("function_declarations")]
        public FunctionDeclaration[]? FunctionDeclarations { get; set; }
    }

    public class FunctionDeclaration
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("parameters")]
        public Parameters Parameters { get; set; } = new();
    }

    public class Parameters
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("properties")]
        public object Properties { get; set; } = new();
        [JsonPropertyName("required")]
        public string[] Required { get; set; } = Array.Empty<string>();
    }
}
