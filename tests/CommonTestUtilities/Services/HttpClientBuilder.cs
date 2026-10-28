using Moq;
using Moq.Protected;
using System.Net;

namespace CommonTestUtilities.Services;

public class HttpClientBuilder
{
    private readonly Mock<HttpMessageHandler> _handlerMock = new(MockBehavior.Strict);

    public HttpClientBuilder SetupGetByteArray(byte[] content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new ByteArrayContent(content)
            });

        return this;
    }

    public HttpClient Build()
    {
        return new HttpClient(_handlerMock.Object);
    }
}
