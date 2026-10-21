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
}
