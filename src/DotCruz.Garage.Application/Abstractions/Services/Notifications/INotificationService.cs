namespace DotCruz.Garage.Application.Abstractions.Services.Notifications;

public interface INotificationService
{
    Task SendMaintenanceAlertAsync(
        Guid userId,
        Guid workspaceId,
        string userName,
        string userEmail,
        string vehicleName,
        string maintenanceDescription,
        int? dueOdometer,
        DateTimeOffset? dueDate,
        CancellationToken cancellationToken);

    Task SendTrafficFineAlertAsync(
        Guid userId,
        Guid workspaceId,
        string userName,
        string userEmail,
        string vehicleName,
        string infractionDescription,
        decimal amount,
        DateTimeOffset dueDate,
        CancellationToken cancellationToken);

    Task SendDocumentAlertAsync(
        Guid userId,
        Guid workspaceId,
        string userName,
        string userEmail,
        string vehicleName,
        string documentType,
        string alertType,
        DateTimeOffset? dueDate,
        DateTimeOffset? expirationDate,
        CancellationToken cancellationToken);

    Task SendOdometerCheckinRequestAsync(
        Guid userId,
        Guid workspaceId,
        string userName,
        string userEmail,
        string vehicleName,
        int currentOdometer,
        string magicLinkUrl,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken);

    Task SendPostSaleWelcomeAsync(
        Guid userId,
        Guid workspaceId,
        string customerName,
        string customerEmail,
        string vehicleName,
        string portalUrl,
        DateTimeOffset? warrantyUntil,
        int? warrantyKm,
        CancellationToken cancellationToken);
}
