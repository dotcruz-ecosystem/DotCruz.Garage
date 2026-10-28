using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.Enums;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.ResponseFormatsRequest;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client;

public interface IGeminiClient
{
    IGeminiClient InitRequest(string model, string input);
    IGeminiClient InitRequest(string input);
    IGeminiClient InitMultimodalRequest(string model, string prompt, byte[] mediaBytes, string mimeType);
    IGeminiClient InitMultimodalRequest(string prompt, byte[] mediaBytes, string mimeType);
    IGeminiClient SetSearchTool();
    IGeminiClient SetPropertyToResponseFormat(string propertyName, ResponseFormatSchemaTypeEnum type, string? description = null);
    IGeminiClient SetArrayPropertyToResponseFormat(string propertyName, ResponseFormatPropertyRequest itemSchema, string? description = null);
    IGeminiClient SetRequiredPropertyToResponseFormat(string propertyName);
    IGeminiClient SetRequiredPropertyToResponseFormat(IEnumerable<string> propertiesName);
    Task<TResponse> MakeRequest<TResponse>(CancellationToken cancellationToken);
}
