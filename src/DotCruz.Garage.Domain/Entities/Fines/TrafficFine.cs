using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.Fines;

public class TrafficFine : BaseEntity, ITenantEntity
{
    public const int DefaultWarningThresholdDays = 10;

    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public string? InfractionCode { get; private set; }
    public string Description { get; private set; } = null!;
    public DateTimeOffset InfractionDate { get; private set; }
    public DateTimeOffset? NotificationDate { get; private set; }
    public DateTimeOffset DueDate { get; private set; }
    public decimal Amount { get; private set; }
    public decimal? DiscountAmount { get; private set; }
    public FineStatus Status { get; private set; }
    public string? ReceiptUrl { get; private set; }
    public DateTimeOffset? LastNotifiedAt { get; private set; }
    public int? WarningThresholdDays { get; private set; }

    public virtual Vehicle Vehicle { get; private init; } = default!;

    private TrafficFine() { }

    public static TrafficFine Create(
        Guid tenantId,
        Guid userId,
        Guid vehicleId,
        string? infractionCode,
        string description,
        DateTimeOffset infractionDate,
        DateTimeOffset? notificationDate,
        DateTimeOffset dueDate,
        decimal amount,
        decimal? discountAmount,
        FineStatus status,
        string? receiptUrl,
        int? warningThresholdDays)
    {
        return new TrafficFine
        {
            TenantId = tenantId,
            UserId = userId,
            VehicleId = vehicleId,
            InfractionCode = infractionCode,
            Description = description,
            InfractionDate = infractionDate,
            NotificationDate = notificationDate,
            DueDate = dueDate,
            Amount = amount,
            DiscountAmount = discountAmount,
            Status = status,
            ReceiptUrl = receiptUrl,
            WarningThresholdDays = warningThresholdDays
        };
    }

    public void Pay(string receiptUrl)
    {
        Status = FineStatus.Paid;
        ReceiptUrl = receiptUrl;
    }

    public void Notify()
    {
        LastNotifiedAt = DateTimeOffset.UtcNow;
    }
}
