using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.Enums;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.ResponseFormatsRequest;
using Moq;

namespace CommonTestUtilities.Services;

public class GeminiClientBuilder
{
    private readonly Mock<IGeminiClient> _mock = new();

    public GeminiClientBuilder()
    {
        _mock.Setup(c => c.InitRequest(It.IsAny<string>(), It.IsAny<string>())).Returns(_mock.Object);
        _mock.Setup(c => c.InitRequest(It.IsAny<string>())).Returns(_mock.Object);
        _mock.Setup(c => c.SetSearchTool()).Returns(_mock.Object);
        _mock.Setup(c => c.SetPropertyToResponseFormat(It.IsAny<string>(), It.IsAny<ResponseFormatSchemaTypeEnum>(), It.IsAny<string?>())).Returns(_mock.Object);
        _mock.Setup(c => c.SetArrayPropertyToResponseFormat(It.IsAny<string>(), It.IsAny<ResponseFormatPropertyRequest>(), It.IsAny<string?>())).Returns(_mock.Object);
        _mock.Setup(c => c.SetRequiredPropertyToResponseFormat(It.IsAny<string>())).Returns(_mock.Object);
        _mock.Setup(c => c.SetRequiredPropertyToResponseFormat(It.IsAny<IEnumerable<string>>())).Returns(_mock.Object);
    }

    public Mock<IGeminiClient> Mock => _mock;

    public GeminiClientBuilder SetupMakeRequest<TResponse>(TResponse response)
    {
        _mock.Setup(c => c.MakeRequest<TResponse>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        return this;
    }

    public GeminiClientBuilder SetupMakeRequestThrows<TException>(TException exception) where TException : Exception
    {
        _mock.Setup(c => c.MakeRequest<It.IsAnyType>(It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
        return this;
    }

    public IGeminiClient Build() => _mock.Object;
}
