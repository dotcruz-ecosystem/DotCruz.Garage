using DotCruz.Garage.Application.Abstractions.Services.Notifications;
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
        var message = new CreateNotificationPayload(
            Type: "Email",
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
            },
            WorkspaceId: workspaceId
        );

        await SendAsync(message, workspaceId, cancellationToken);
    }

    public async Task SendBlueprintReadyNotificationAsync(
        Guid userId,
        Guid workspaceId,
        string userName,
        string userEmail,
        Guid vehicleId,
        string vehicleName,
        int templatesCount,
        CancellationToken cancellationToken)
    {
        // 1. In-App Notification (Sininho no TopBar do ecossistema)
        var inAppMessage = new CreateNotificationPayload(
            Type: "InApp",
            Recipient: userId.ToString(),
            Title: "Plano de Manutenção Disponível 🛠️",
            Body: $"Identificamos o manual da sua {vehicleName} e montamos {templatesCount} recomendações preventivas sob medida!",
            SourceApp: "garage",
            ActionUrl: $"http://localhost:3004/vehicles/{vehicleId}?openBlueprint=true",
            WorkspaceId: workspaceId
        );

        // 2. Email Notification (Template Oficial VehicleBlueprintReady)
        var emailMessage = new CreateNotificationPayload(
            Type: "Email",
            Recipient: userEmail,
            Culture: CultureInfo.CurrentUICulture.Name,
            TemplateCode: "VehicleBlueprintReady",
            TemplateData: new Dictionary<string, object>
            {
                { "name", userName },
                { "vehicle_name", vehicleName },
                { "items_count", templatesCount },
                { "vehicle_url", $"http://localhost:3004/vehicles/{vehicleId}?openBlueprint=true" }
            },
            WorkspaceId: workspaceId
        );

        await Task.WhenAll(
            SendAsync(inAppMessage, workspaceId, cancellationToken),
            SendAsync(emailMessage, workspaceId, cancellationToken)
        );
    }

    private async Task SendAsync(CreateNotificationPayload message, Guid workspaceId, CancellationToken cancellationToken)
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

    private record CreateNotificationPayload(
        string Type,
        string Recipient,
        string? Title = null,
        string? Body = null,
        string? Culture = null,
        string? TemplateCode = null,
        Dictionary<string, object>? TemplateData = null,
        DateTimeOffset? ScheduledFor = null,
        Guid? WorkspaceId = null,
        string? SourceApp = null,
        string? ActionUrl = null
    );
}
