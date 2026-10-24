using DotCruz.Garage.Application.Abstractions.Services.Notifications;
using Moq;

namespace CommonTestUtilities.Services;

public class NotificationServiceBuilder
{
    private readonly Mock<INotificationService> _service = new();

    public Mock<INotificationService> Mock => _service;

    public NotificationServiceBuilder VerifyBlueprintReadyNotificationSent(Guid userId, Guid vehicleId, Times times)
    {
        _service.Verify(s => s.SendBlueprintReadyNotificationAsync(
            userId,
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            vehicleId,
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()), times);

        return this;
    }

    public INotificationService Build() => _service.Object;
}
