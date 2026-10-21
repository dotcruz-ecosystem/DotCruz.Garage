namespace DotCruz.Garage.Application.DTOs.Maintenance;

public sealed record CreateMaintenanceAttachmentDto(
    string FileName,
    string FileUrl,
    string ContentType,
    long FileSize
);
