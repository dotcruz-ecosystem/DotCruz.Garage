namespace DotCruz.Garage.Application.DTOs.Maintenance;

public sealed record CreateLogMaintenanceRequestDto(
    Guid? MaintenanceScheduleId,
    string Description,
    DateTimeOffset PerformedAt,
    int OdometerAtService,
    decimal Cost,
    string? ReceiptUrl = null,
    string? Notes = null,
    List<CreateMaintenanceAttachmentDto>? Attachments = null
);
