using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.Maintenance;

public class MaintenanceLog : BaseEntity, IWorkspaceEntity
{
    public Guid WorkspaceId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid? MaintenanceScheduleId { get; private set; }
    public string Description { get; private set; } = null!;
    public DateTimeOffset PerformedAt { get; private set; }
    public int OdometerAtService { get; private set; }
    public decimal Cost { get; private set; }
    public string? ReceiptUrl { get; private set; }
    public string? Notes { get; private set; }
    public MaintenanceType Type => MaintenanceScheduleId.HasValue ? MaintenanceType.Preventive : MaintenanceType.Corrective;

    public virtual ICollection<MaintenanceAttachment> Attachments { get; private init; } = new List<MaintenanceAttachment>();

    private MaintenanceLog() { }

    public static MaintenanceLog Create(
        Guid workspaceId,
        Guid userId,
        Guid vehicleId,
        Guid? maintenanceScheduleId,
        string description,
        DateTimeOffset performedAt,
        int odometerAtService,
        decimal cost,
        string? receiptUrl,
        string? notes)
    {
        return new MaintenanceLog
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            VehicleId = vehicleId,
            MaintenanceScheduleId = maintenanceScheduleId,
            Description = description,
            PerformedAt = performedAt,
            OdometerAtService = odometerAtService,
            Cost = cost,
            ReceiptUrl = receiptUrl,
            Notes = notes
        };
    }

    public void UpdateReceiptUrl(string receiptUrl)
    {
        ReceiptUrl = receiptUrl;
        Touch();
    }

    public MaintenanceAttachment AddAttachment(string fileName, string fileUrl, string contentType, long fileSize)
    {
        var attachment = MaintenanceAttachment.Create(Id, fileName, fileUrl, contentType, fileSize);
        Attachments.Add(attachment);
        Touch();
        return attachment;
    }

    public void RemoveAttachment(Guid attachmentId)
    {
        var attachment = Attachments.FirstOrDefault(a => a.Id == attachmentId);
        if (attachment != null)
        {
            Attachments.Remove(attachment);
            Touch();
        }
    }
}
