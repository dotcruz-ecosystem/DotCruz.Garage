using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.Enums;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.ResponseFormatsRequest;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.ToolsRequest;
using System.Text.Json.Serialization;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests;

public class GeminiApiBaseRequest
{
    [JsonPropertyName("model")]
    public string Model { get; private set; }

    [JsonPropertyName("input")]
    public object Input { get; private set; }

    public GeminiApiBaseRequest(string model, string input)
    {
        Model = model;
        Input = input;
    }

    public GeminiApiBaseRequest(string model, object input)
    {
        Model = model;
        Input = input;
    }

    [JsonPropertyName("tools")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<BaseToolRequest>? Tools { get; set; } = null;

    [JsonPropertyName("response_format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ResponseFormatRequest? ResponseFormat { get; set; }

    public GeminiApiBaseRequest AddToolToRequest(ToolTypeEnum toolType)
    {
        Tools ??= [];
        Tools.Add(new ToolRequest(toolType));
        return this;
    }

    public GeminiApiBaseRequest AddResponseFormatProperty(string propertyName, ResponseFormatSchemaTypeEnum type, string? description = null)
    {
        ResponseFormat ??= new();
        ResponseFormat.Schema.AddProperty(propertyName, type, description);
        return this;
    }

    public GeminiApiBaseRequest AddResponseFormatArrayProperty(string propertyName, ResponseFormatPropertyRequest itemSchema, string? description = null)
    {
        ResponseFormat ??= new();
        ResponseFormat.Schema.AddArrayProperty(propertyName, itemSchema, description);
        return this;
    }

    public GeminiApiBaseRequest AddResponseFormatRequiredProperty(string propertyName)
    {
        ResponseFormat ??= new();
        ResponseFormat.Schema.AddRequiredProperty(propertyName);
        return this;
    }

    public GeminiApiBaseRequest AddResponseFormatRequiredProperty(IEnumerable<string> propertiesName)
    {
        ResponseFormat ??= new();
        ResponseFormat.Schema.AddRequiredProperty(propertiesName);
        return this;
    }
}
