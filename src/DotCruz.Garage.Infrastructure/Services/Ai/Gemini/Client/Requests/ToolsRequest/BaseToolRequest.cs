using System.Text.Json.Serialization;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.ToolsRequest;

public abstract class BaseToolRequest(string type)
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = type;
}
