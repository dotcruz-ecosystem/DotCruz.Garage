namespace DotCruz.Garage.Application.Abstractions.Services.Notifications;

public interface INotificationService
{
    Task SendMaintenanceAlertAsync(
        Guid userId,
        string userName,
        string userEmail,
        string vehicleName,
        string maintenanceDescription,
        int? dueOdometer,
        DateTimeOffset? dueDate,
        CancellationToken cancellationToken);

    Task SendTrafficFineAlertAsync(
        Guid userId,
        string userName,
        string userEmail,
        string vehicleName,
        string infractionDescription,
        decimal amount,
        DateTimeOffset dueDate,
        CancellationToken cancellationToken);

    Task SendDocumentAlertAsync(
        Guid userId,
        string userName,
        string userEmail,
        string vehicleName,
        string documentType,
        string alertType,
        DateTimeOffset? dueDate,
        DateTimeOffset? expirationDate,
        CancellationToken cancellationToken);
}
