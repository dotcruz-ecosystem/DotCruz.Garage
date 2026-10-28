using System.Text.Json.Serialization;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests;

public class GeminiContentPart
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Data { get; set; }

    [JsonPropertyName("mime_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MimeType { get; set; }

    public static GeminiContentPart TextPart(string text) => new()
    {
        Type = "text",
        Text = text
    };

    public static GeminiContentPart MediaPart(byte[] data, string mimeType)
    {
        var normalizedMime = mimeType.ToLowerInvariant();
        var type = normalizedMime.Contains("pdf") ? "document" : "image";

        return new()
        {
            Type = type,
            Data = Convert.ToBase64String(data),
            MimeType = normalizedMime
        };
    }
}
