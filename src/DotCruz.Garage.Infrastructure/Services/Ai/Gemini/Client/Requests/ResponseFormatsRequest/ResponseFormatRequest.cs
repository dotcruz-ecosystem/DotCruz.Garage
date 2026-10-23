using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.Enums;
using System.Text.Json.Serialization;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.ResponseFormatsRequest;

public class ResponseFormatRequest
{
    [JsonPropertyName("type")]
    public ResponseFormatTypeEnum Type { get; set; } = ResponseFormatTypeEnum.Text;

    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; } = "application/json";

    [JsonPropertyName("schema")]
    public ResponseFormatSchemaRequest Schema { get; set; } = new();
}
