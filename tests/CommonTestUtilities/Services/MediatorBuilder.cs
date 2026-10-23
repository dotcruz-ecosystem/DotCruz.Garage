using MediatR;
using Moq;

namespace CommonTestUtilities.Services;

public class MediatorBuilder
{
    private readonly Mock<IMediator> _mock = new();

    public Mock<IMediator> Mock => _mock;

    public MediatorBuilder SetupSend<TResponse>(TResponse response)
    {
        _mock.Setup(m => m.Send(It.IsAny<IRequest<TResponse>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        return this;
    }

    public IMediator Build()
    {
        return _mock.Object;
    }
}
