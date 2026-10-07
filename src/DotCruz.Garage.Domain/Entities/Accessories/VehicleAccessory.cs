using DotCruz.Garage.Domain.Entities.Base;

namespace DotCruz.Garage.Domain.Entities.Accessories;

public class VehicleAccessory : BaseEntity, IWorkspaceEntity
{
    public Guid WorkspaceId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Store { get; private set; }
    public DateTimeOffset PurchaseDate { get; private set; }
    public decimal Cost { get; private set; }
    public string? ReceiptUrl { get; private set; }
    public string? Notes { get; private set; }

    private VehicleAccessory() { }

    public static VehicleAccessory Create(
        Guid workspaceId,
        Guid userId,
        Guid vehicleId,
        string name,
        string? store,
        DateTimeOffset purchaseDate,
        decimal cost,
        string? receiptUrl,
        string? notes)
    {
        return new VehicleAccessory
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            VehicleId = vehicleId,
            Name = name,
            Store = store,
            PurchaseDate = purchaseDate,
            Cost = cost,
            ReceiptUrl = receiptUrl,
            Notes = notes
        };
    }
}
