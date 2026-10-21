namespace DotCruz.Garage.Application.DTOs.Maintenance;

public record MaintenanceLogDto(
    Guid Id,
    Guid VehicleId,
    Guid? MaintenanceScheduleId,
    string Description,
    DateTimeOffset PerformedAt,
    int OdometerAtService,
    decimal Cost,
    string? ReceiptUrl,
    string? Notes,
    string Type,
    IReadOnlyList<MaintenanceAttachmentDto> Attachments
);
