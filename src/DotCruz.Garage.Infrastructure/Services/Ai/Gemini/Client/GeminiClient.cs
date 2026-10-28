using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.Enums;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.ResponseFormatsRequest;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client;

public class GeminiClient(HttpClient httpClient) : IGeminiClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    public GeminiApiBaseRequest? BaseRequest { get; private set; }

    public IGeminiClient InitRequest(string model, string input)
    {
        BaseRequest = new GeminiApiBaseRequest(model, input);
        return this;
    }

    public IGeminiClient InitRequest(string input)
    {
        return InitRequest("gemini-3.8-flash", input);
    }

    public IGeminiClient InitMultimodalRequest(string model, string prompt, byte[] mediaBytes, string mimeType)
    {
        var parts = new List<GeminiContentPart>
        {
            GeminiContentPart.TextPart(prompt),
            GeminiContentPart.MediaPart(mediaBytes, mimeType)
        };

        BaseRequest = new GeminiApiBaseRequest(model, parts);
        return this;
    }

    public IGeminiClient InitMultimodalRequest(string prompt, byte[] mediaBytes, string mimeType)
    {
        return InitMultimodalRequest("gemini-3.8-flash", prompt, mediaBytes, mimeType);
    }

    public IGeminiClient SetSearchTool()
    {
        MakeSureBaseRequestInit();
        BaseRequest!.AddToolToRequest(ToolTypeEnum.GoogleSearch);
        return this;
    }

    public IGeminiClient SetPropertyToResponseFormat(string propertyName, ResponseFormatSchemaTypeEnum type, string? description = null)
    {
        MakeSureBaseRequestInit();
        BaseRequest!.AddResponseFormatProperty(propertyName, type, description);
        return this;
    }

    public IGeminiClient SetArrayPropertyToResponseFormat(string propertyName, ResponseFormatPropertyRequest itemSchema, string? description = null)
    {
        MakeSureBaseRequestInit();
        BaseRequest!.AddResponseFormatArrayProperty(propertyName, itemSchema, description);
        return this;
    }

    public IGeminiClient SetRequiredPropertyToResponseFormat(string propertyName)
    {
        MakeSureBaseRequestInit();
        BaseRequest!.AddResponseFormatRequiredProperty(propertyName);
        return this;
    }

    public IGeminiClient SetRequiredPropertyToResponseFormat(IEnumerable<string> propertiesName)
    {
        MakeSureBaseRequestInit();
        BaseRequest!.AddResponseFormatRequiredProperty(propertiesName);
        return this;
    }

    public async Task<TResponse> MakeRequest<TResponse>(CancellationToken cancellationToken)
    {
        MakeSureBaseRequestInit();

        var endpoint = httpClient.BaseAddress == null ? "https://generativelanguage.googleapis.com/v1beta/interactions" : "interactions";

        var response = await httpClient.PostAsJsonAsync(endpoint, BaseRequest, SerializerOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new GeminiApiException(response.StatusCode, errorBody, $"Gemini API returned HTTP {response.StatusCode}");
        }

        var interactionResponse = await response.Content.ReadFromJsonAsync<GeminiInteractionResponse>(SerializerOptions, cancellationToken);
        var rawText = interactionResponse?.Steps?
            .Where(s => s.Type == "model_output")
            .SelectMany(s => s.Content ?? [])
            .LastOrDefault(c => c.Type == "text" || !string.IsNullOrWhiteSpace(c.Text))?.Text;

        if (string.IsNullOrWhiteSpace(rawText))
        {
            throw new GeminiApiException(response.StatusCode, "Gemini interaction response did not contain output text in steps.", "Gemini interaction response did not contain output text in steps.");
        }

        var cleanedJson = CleanMarkdownCodeBlocks(rawText);

        var result = JsonSerializer.Deserialize<TResponse>(cleanedJson, SerializerOptions);
        if (result == null)
        {
            throw new GeminiApiException(response.StatusCode, $"Failed to deserialize Gemini output text to {typeof(TResponse).Name}. Raw: {cleanedJson}");
        }

        return result;
    }

    private void MakeSureBaseRequestInit()
    {
        if (BaseRequest == null)
            throw new InvalidStateException(ResourceMessagesException.GEMINI_CLIENT_BASE_REQUEST_NOT_INITALIZED);
    }

    private static string CleanMarkdownCodeBlocks(string text)
    {
        var cleaned = text.Trim();
        if (cleaned.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned[7..];
        }
        else if (cleaned.StartsWith("```"))
        {
            cleaned = cleaned[3..];
        }

        if (cleaned.EndsWith("```"))
        {
            cleaned = cleaned[..^3];
        }

        return cleaned.Trim();
    }

    private class GeminiInteractionResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("steps")]
        public List<GeminiInteractionStep>? Steps { get; set; }
    }

    private class GeminiInteractionStep
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("content")]
        public List<GeminiInteractionContent>? Content { get; set; }
    }

    private class GeminiInteractionContent
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
