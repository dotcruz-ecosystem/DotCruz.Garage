using DotCruz.Garage.Domain.Entities.Base;

namespace DotCruz.Garage.Domain.Entities.Maintenance;

public class MaintenanceAttachment : BaseEntity
{
    public Guid MaintenanceLogId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string FileUrl { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long FileSize { get; private set; }

    private MaintenanceAttachment() { }

    public static MaintenanceAttachment Create(
        Guid maintenanceLogId,
        string fileName,
        string fileUrl,
        string contentType,
        long fileSize)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        return new MaintenanceAttachment
        {
            MaintenanceLogId = maintenanceLogId,
            FileName = fileName.Trim(),
            FileUrl = fileUrl.Trim(),
            ContentType = contentType.Trim().ToLowerInvariant(),
            FileSize = fileSize
        };
    }
}
