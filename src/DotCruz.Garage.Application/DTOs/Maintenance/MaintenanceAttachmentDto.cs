namespace DotCruz.Garage.Application.DTOs.Maintenance;

public record MaintenanceAttachmentDto(
    Guid Id,
    Guid MaintenanceLogId,
    string FileName,
    string FileUrl,
    string ContentType,
    long FileSize,
    DateTimeOffset CreatedAt
);
