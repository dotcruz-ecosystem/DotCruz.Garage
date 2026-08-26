using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Entities.Drivers;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.Fines;

public class TrafficFine : BaseEntity, ITenantEntity
{
    public const int DefaultWarningThresholdDays = 10;
    public const int NICRiskWarningThresholdDays = 15;

    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid? DriverId { get; private set; }
    public string? InfractionCode { get; private set; }
    public string? IssuingAuthority { get; private set; }
    public string Description { get; private set; } = null!;
    public int Points { get; private set; }
    public DateTimeOffset InfractionDate { get; private set; }
    public DateTimeOffset? NotificationDate { get; private set; }
    public DateTimeOffset DueDate { get; private set; }
    public DateTimeOffset? IndicationDeadline { get; private set; }
    public DateTimeOffset? IndicatedAt { get; private set; }
    public string? IndicationProtocol { get; private set; }
    public decimal Amount { get; private set; }
    public decimal? DiscountAmount { get; private set; }
    public FineStatus Status { get; private set; }
    public string? ReceiptUrl { get; private set; }
    public DateTimeOffset? LastNotifiedAt { get; private set; }
    public int? WarningThresholdDays { get; private set; }

    public virtual Vehicle Vehicle { get; private init; } = default!;
    public virtual Driver? Driver { get; private init; }

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
        int? warningThresholdDays,
        Guid? driverId = null,
        string? issuingAuthority = null,
        int points = 0,
        DateTimeOffset? indicationDeadline = null,
        string? indicationProtocol = null)
    {
        return new TrafficFine
        {
            TenantId = tenantId,
            UserId = userId,
            VehicleId = vehicleId,
            DriverId = driverId,
            InfractionCode = infractionCode,
            IssuingAuthority = issuingAuthority,
            Description = description,
            Points = points,
            InfractionDate = infractionDate,
            NotificationDate = notificationDate,
            DueDate = dueDate,
            IndicationDeadline = indicationDeadline,
            IndicationProtocol = indicationProtocol,
            Amount = amount,
            DiscountAmount = discountAmount,
            Status = status,
            ReceiptUrl = receiptUrl,
            WarningThresholdDays = warningThresholdDays
        };
    }

    public void IndicateDriver(Guid driverId, string? protocol = null)
    {
        DriverId = driverId;
        IndicatedAt = DateTimeOffset.UtcNow;
        IndicationProtocol = protocol;
        if (Status == FineStatus.Pending || Status == FineStatus.NICRisk)
        {
            Status = FineStatus.Indicated;
        }
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

    public int? DaysUntilIndicationDeadline(DateTimeOffset? now = null)
    {
        if (!IndicationDeadline.HasValue)
            return null;

        var refDate = now ?? DateTimeOffset.UtcNow;
        return (int)Math.Ceiling((IndicationDeadline.Value - refDate).TotalDays);
    }

    public bool IsNICRisk(DateTimeOffset? now = null)
    {
        if (Status == FineStatus.Paid || Status == FineStatus.Indicated || IndicatedAt != null)
            return false;

        var days = DaysUntilIndicationDeadline(now);
        return days.HasValue && days.Value <= NICRiskWarningThresholdDays && days.Value >= 0;
    }
}
