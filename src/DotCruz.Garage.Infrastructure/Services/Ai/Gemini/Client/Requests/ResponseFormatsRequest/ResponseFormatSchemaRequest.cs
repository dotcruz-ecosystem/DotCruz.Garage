using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.Enums;
using System.Text.Json.Serialization;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.ResponseFormatsRequest;

public class ResponseFormatSchemaRequest
{
    [JsonPropertyName("type")]
    public ResponseFormatSchemaTypeEnum Type { get; set; } = ResponseFormatSchemaTypeEnum.Object;

    [JsonPropertyName("properties")]
    public Dictionary<string, ResponseFormatPropertyRequest> Properties { get; set; } = [];

    [JsonPropertyName("required")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> Required { get; set; } = [];

    public ResponseFormatSchemaRequest AddProperty(string propertyName, ResponseFormatSchemaTypeEnum type, string? description = null)
    {
        Properties.TryAdd(propertyName, new ResponseFormatPropertyRequest(type, description));
        return this;
    }

    public ResponseFormatSchemaRequest AddProperty(string propertyName, ResponseFormatPropertyRequest propertySchema)
    {
        Properties.TryAdd(propertyName, propertySchema);
        return this;
    }

    public ResponseFormatSchemaRequest AddArrayProperty(string propertyName, ResponseFormatPropertyRequest itemSchema, string? description = null)
    {
        Properties.TryAdd(propertyName, ResponseFormatPropertyRequest.CreateArray(itemSchema, description));
        return this;
    }

    public ResponseFormatSchemaRequest AddRequiredProperty(string property)
    {
        Required.Add(property);
        return this;
    }

    public ResponseFormatSchemaRequest AddRequiredProperty(IEnumerable<string> properties)
    {
        Required.AddRange(properties);
        return this;
    }
}
