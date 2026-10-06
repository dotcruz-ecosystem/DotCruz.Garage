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
        Guid tenantId,
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

        await SendAsync(message, tenantId, cancellationToken);
    }

    public async Task SendTrafficFineAlertAsync(
        Guid userId,
        Guid tenantId,
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

        await SendAsync(message, tenantId, cancellationToken);
    }

    public async Task SendDocumentAlertAsync(
        Guid userId,
        Guid tenantId,
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

        await SendAsync(message, tenantId, cancellationToken);
    }

    public async Task SendOdometerCheckinRequestAsync(
        Guid userId,
        Guid tenantId,
        string userName,
        string userEmail,
        string vehicleName,
        int currentOdometer,
        string magicLinkUrl,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken)
    {
        var message = new CreateNotificationMessage(
            Type: IntegrationNotificationType.Email,
            Recipient: userEmail,
            Culture: CultureInfo.CurrentUICulture.Name,
            TemplateCode: "OdometerCheckinAlert",
            TemplateData: new Dictionary<string, object>
            {
                { "name", userName },
                { "vehicle_name", vehicleName },
                { "current_odometer", currentOdometer },
                { "magic_link_url", magicLinkUrl },
                { "expires_at", expiresAt.ToString("yyyy-MM-dd HH:mm") }
            }
        );

        await SendAsync(message, tenantId, cancellationToken);
    }

    public async Task SendPostSaleWelcomeAsync(
        Guid userId,
        Guid tenantId,
        string customerName,
        string customerEmail,
        string vehicleName,
        string portalUrl,
        DateTimeOffset? warrantyUntil,
        int? warrantyKm,
        CancellationToken cancellationToken)
    {
        var message = new CreateNotificationMessage(
            Type: IntegrationNotificationType.Email,
            Recipient: customerEmail,
            Culture: CultureInfo.CurrentUICulture.Name,
            TemplateCode: "VehiclePostSaleWelcome",
            TemplateData: new Dictionary<string, object>
            {
                { "name", customerName },
                { "vehicle_name", vehicleName },
                { "portal_url", portalUrl },
                { "warranty_until", warrantyUntil?.ToString("yyyy-MM-dd") ?? string.Empty },
                { "warranty_km", warrantyKm ?? 0 }
            }
        );

        await SendAsync(message, tenantId, cancellationToken);
    }

    private async Task SendAsync(CreateNotificationMessage message, Guid tenantId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/Notification")
        {
            Content = JsonContent.Create(message, options: JsonOptions)
        };

        if (tenantId != Guid.Empty)
            request.Headers.Add("X-Tenant-ID", tenantId.ToString());

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
