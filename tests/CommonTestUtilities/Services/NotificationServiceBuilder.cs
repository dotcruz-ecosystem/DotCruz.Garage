using DotCruz.Garage.Application.Abstractions.Services.Notifications;
using Moq;

namespace CommonTestUtilities.Services;

public class NotificationServiceBuilder
{
    private readonly Mock<INotificationService> _service = new();

    public Mock<INotificationService> Mock => _service;

    public INotificationService Build() => _service.Object;
}
