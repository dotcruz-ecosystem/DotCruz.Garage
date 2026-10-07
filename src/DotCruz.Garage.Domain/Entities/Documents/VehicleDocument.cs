using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.Documents;

public class VehicleDocument : BaseEntity, IWorkspaceEntity
{
    public const int DefaultWarningThresholdDays = 30;

    public Guid WorkspaceId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public DocumentType Type { get; private set; }
    public int ReferenceYear { get; private set; }
    public DateTimeOffset? DueDate { get; private set; }
    public decimal Cost { get; private set; }
    public DateTimeOffset? ExpirationDate { get; private set; }
    public DocumentStatus Status { get; private set; }
    public string? FileUrl { get; private set; }
    public DateTimeOffset? LastNotifiedAt { get; private set; }
    public DocumentStatus? LastNotifiedStatus { get; private set; }
    public int? WarningThresholdDays { get; private set; }

    public Vehicle Vehicle { get; private init; } = default!;

    private VehicleDocument() { }

    public static VehicleDocument Create(
        Guid workspaceId,
        Guid userId,
        Guid vehicleId,
        DocumentType type,
        int referenceYear,
        DateTimeOffset? dueDate,
        decimal cost,
        DateTimeOffset? expirationDate,
        DocumentStatus status,
        string? fileUrl,
        int? warningThresholdDays)
    {
        return new VehicleDocument
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            VehicleId = vehicleId,
            Type = type,
            ReferenceYear = referenceYear,
            DueDate = dueDate,
            Cost = cost,
            ExpirationDate = expirationDate,
            Status = status,
            FileUrl = fileUrl,
            WarningThresholdDays = warningThresholdDays
        };
    }

    public void Pay(string fileUrl)
    {
        Status = DocumentStatus.Paid;
        FileUrl = fileUrl;
    }

    public void Notify(DocumentStatus status)
    {
        LastNotifiedAt = DateTimeOffset.UtcNow;
        LastNotifiedStatus = status;
    }
}
