using DotCruz.Garage.Application.Abstractions.Services.Notifications;
using DotCruz.Notifications.Contracts.Enums.Notifications;
using DotCruz.Notifications.Contracts.Messages.Notifications.CreateNotification;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotCruz.Garage.Infrastructure.Services.Notifications;

public class NotificationService(HttpClient httpClient) : INotificationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SendMaintenanceAlertAsync(
        Guid userId,
        string userName,
        string userEmail,
        string vehicleName,
        string maintenanceDescription,
        int? dueOdometer,
        DateTimeOffset? dueDate,
        CancellationToken cancellationToken)
    {
        var message = new CreateNotificationMessage(
            Type: IntegrationNotificationType.Email,
            Recipient: userEmail,
            Culture: CultureInfo.CurrentUICulture.Name,
            TemplateCode: "VehicleMaintenanceAlert",
            TemplateData: new Dictionary<string, object>
            {
                { "name", userName },
                { "vehicle_name", vehicleName },
                { "description", maintenanceDescription },
                { "due_odometer", dueOdometer ?? 0 },
                { "due_date", dueDate?.ToString("yyyy-MM-dd") ?? string.Empty }
            }
        );

        var response = await httpClient.PostAsJsonAsync("api/Notification", message, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task SendTrafficFineAlertAsync(
        Guid userId,
        string userName,
        string userEmail,
        string vehicleName,
        string infractionDescription,
        decimal amount,
        DateTimeOffset dueDate,
        CancellationToken cancellationToken)
    {
        var message = new CreateNotificationMessage(
            Type: IntegrationNotificationType.Email,
            Recipient: userEmail,
            Culture: CultureInfo.CurrentUICulture.Name,
            TemplateCode: "NewTrafficFineAlert",
            TemplateData: new Dictionary<string, object>
            {
                { "name", userName },
                { "vehicle_name", vehicleName },
                { "description", infractionDescription },
                { "amount", amount },
                { "due_date", dueDate.ToString("yyyy-MM-dd") }
            }
        );

        var response = await httpClient.PostAsJsonAsync("api/Notification", message, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task SendDocumentAlertAsync(
        Guid userId,
        string userName,
        string userEmail,
        string vehicleName,
        string documentType,
        string alertType,
        DateTimeOffset? dueDate,
        DateTimeOffset? expirationDate,
        CancellationToken cancellationToken)
    {
        var message = new CreateNotificationMessage(
            Type: IntegrationNotificationType.Email,
            Recipient: userEmail,
            Culture: CultureInfo.CurrentUICulture.Name,
            TemplateCode: "VehicleDocumentAlert",
            TemplateData: new Dictionary<string, object>
            {
                { "name", userName },
                { "vehicle_name", vehicleName },
                { "document_type", documentType },
                { "alert_type", alertType },
                { "due_date", dueDate?.ToString("yyyy-MM-dd") ?? string.Empty },
                { "expiration_date", expirationDate?.ToString("yyyy-MM-dd") ?? string.Empty }
            }
        );

        var response = await httpClient.PostAsJsonAsync("api/Notification", message, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
