using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.Enums;
using System.Text.Json.Serialization;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.ResponseFormatsRequest;

public class ResponseFormatPropertyRequest
{
    [JsonPropertyName("type")]
    public ResponseFormatSchemaTypeEnum Type { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ResponseFormatPropertyRequest? Items { get; set; }

    [JsonPropertyName("properties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, ResponseFormatPropertyRequest>? Properties { get; set; }

    [JsonPropertyName("required")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Required { get; set; }

    public ResponseFormatPropertyRequest(ResponseFormatSchemaTypeEnum type, string? description = null)
    {
        Type = type;
        Description = description;
    }

    public ResponseFormatPropertyRequest()
    {
    }

    public static ResponseFormatPropertyRequest CreateObject(string? description = null)
    {
        return new ResponseFormatPropertyRequest(ResponseFormatSchemaTypeEnum.Object, description)
        {
            Properties = [],
            Required = []
        };
    }

    public static ResponseFormatPropertyRequest CreateArray(ResponseFormatPropertyRequest itemsSchema, string? description = null)
    {
        return new ResponseFormatPropertyRequest(ResponseFormatSchemaTypeEnum.Array, description)
        {
            Items = itemsSchema
        };
    }

    public ResponseFormatPropertyRequest AddProperty(string propertyName, ResponseFormatSchemaTypeEnum type, string? description = null)
    {
        Properties ??= [];
        Properties.TryAdd(propertyName, new ResponseFormatPropertyRequest(type, description));
        return this;
    }

    public ResponseFormatPropertyRequest AddProperty(string propertyName, ResponseFormatPropertyRequest propertySchema)
    {
        Properties ??= [];
        Properties.TryAdd(propertyName, propertySchema);
        return this;
    }

    public ResponseFormatPropertyRequest AddRequiredProperty(string propertyName)
    {
        Required ??= [];
        Required.Add(propertyName);
        return this;
    }

    public ResponseFormatPropertyRequest AddRequiredProperty(IEnumerable<string> propertiesName)
    {
        Required ??= [];
        Required.AddRange(propertiesName);
        return this;
    }
}
