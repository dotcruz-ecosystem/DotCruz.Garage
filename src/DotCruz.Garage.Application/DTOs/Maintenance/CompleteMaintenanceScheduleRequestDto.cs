namespace DotCruz.Garage.Application.DTOs.Maintenance;

public record CompleteMaintenanceScheduleRequestDto(
    DateTimeOffset PerformedAt,
    int OdometerAtService,
    decimal Cost,
    string? ReceiptUrl = null,
    string? Notes = null,
    List<CreateMaintenanceAttachmentDto>? Attachments = null);
