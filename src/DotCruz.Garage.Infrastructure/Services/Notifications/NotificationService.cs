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
        Guid workspaceId,
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

        await SendAsync(message, workspaceId, cancellationToken);
    }

    private async Task SendAsync(CreateNotificationMessage message, Guid workspaceId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/Notification")
        {
            Content = JsonContent.Create(message, options: JsonOptions)
        };

        if (workspaceId != Guid.Empty)
            request.Headers.Add("X-Workspace-ID", workspaceId.ToString());

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
