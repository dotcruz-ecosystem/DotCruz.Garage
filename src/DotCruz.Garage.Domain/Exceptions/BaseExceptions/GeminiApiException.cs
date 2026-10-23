using System.Net;
using DotCruz.Garage.Domain.Exceptions.Enums;

namespace DotCruz.Garage.Domain.Exceptions.BaseExceptions;

public class GeminiApiException(HttpStatusCode statusCode, string responseContent, string message = "Gemini API request failed.") 
    : GarageException(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public string ResponseContent { get; } = responseContent;

    public override IEnumerable<string> GetErrorsMessages() => [Message, $"Status: {StatusCode}", ResponseContent];

    public override ErrorType GetErrorType() => ErrorType.Infrastructure;
}
