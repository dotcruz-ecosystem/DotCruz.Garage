namespace DotCruz.Garage.Application.DTOs.Maintenance;

public sealed record GetMaintenanceUploadUrlRequest(string FileName, string ContentType, long FileSize);
