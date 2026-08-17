using DotCruz.Garage.Domain.Entities.Base;

namespace DotCruz.Garage.Domain.Entities.Maintenance;

public class MaintenanceLog : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid? MaintenanceScheduleId { get; private set; }
    public string Description { get; private set; } = null!;
    public DateTimeOffset PerformedAt { get; private set; }
    public int OdometerAtService { get; private set; }
    public decimal Cost { get; private set; }
    public string? ReceiptUrl { get; private set; }
    public string? Notes { get; private set; }

    private MaintenanceLog() { }

    public static MaintenanceLog Create(
        Guid tenantId,
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
            TenantId = tenantId,
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
    }
}
