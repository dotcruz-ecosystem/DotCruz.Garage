using DotCruz.Garage.Domain.Entities.Base;

namespace DotCruz.Garage.Domain.Entities.Vehicles;

public class VehicleSale : BaseEntity, IWorkspaceEntity
{
    public Guid WorkspaceId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public string CustomerName { get; private set; } = null!;
    public string CustomerEmail { get; private set; } = null!;
    public string? CustomerPhone { get; private set; }
    public string? CustomerDocument { get; private set; }
    public DateTimeOffset SoldAt { get; private set; }
    public decimal? SoldPrice { get; private set; }
    public int SoldOdometer { get; private set; }
    public DateTimeOffset? WarrantyExpiresAt { get; private set; }
    public int? WarrantyKm { get; private set; }
    public string? Notes { get; private set; }
    public Guid? ClaimedByUserId { get; private set; }
    public DateTimeOffset? ClaimedAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    public virtual Vehicle Vehicle { get; private init; } = default!;

    private VehicleSale() { }

    public static VehicleSale Create(
        Guid workspaceId,
        Guid userId,
        Guid vehicleId,
        string customerName,
        string customerEmail,
        string? customerPhone,
        string? customerDocument,
        DateTimeOffset soldAt,
        decimal? soldPrice,
        int soldOdometer,
        DateTimeOffset? warrantyExpiresAt,
        int? warrantyKm,
        string? notes)
    {
        if (workspaceId == Guid.Empty)
            throw new ArgumentException("WorkspaceId não pode ser vazio.", nameof(workspaceId));

        if (vehicleId == Guid.Empty)
            throw new ArgumentException("VehicleId não pode ser vazio.", nameof(vehicleId));

        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Nome do cliente não pode ser vazio.", nameof(customerName));

        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("E-mail do cliente não pode ser vazio.", nameof(customerEmail));

        if (soldOdometer < 0)
            throw new ArgumentException("Quilometragem de venda não pode ser negativa.", nameof(soldOdometer));

        return new VehicleSale
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            VehicleId = vehicleId,
            CustomerName = customerName.Trim(),
            CustomerEmail = customerEmail.Trim().ToLowerInvariant(),
            CustomerPhone = string.IsNullOrWhiteSpace(customerPhone) ? null : customerPhone.Trim(),
            CustomerDocument = string.IsNullOrWhiteSpace(customerDocument) ? null : customerDocument.Trim(),
            SoldAt = soldAt,
            SoldPrice = soldPrice,
            SoldOdometer = soldOdometer,
            WarrantyExpiresAt = warrantyExpiresAt,
            WarrantyKm = warrantyKm,
            Notes = notes?.Trim(),
            IsActive = true
        };
    }

    public bool IsWarrantyActive(int currentOdometer, DateTimeOffset now)
    {
        if (!IsActive)
            return false;

        if (WarrantyExpiresAt.HasValue && now > WarrantyExpiresAt.Value)
            return false;

        if (WarrantyKm.HasValue && currentOdometer > (SoldOdometer + WarrantyKm.Value))
            return false;

        return WarrantyExpiresAt.HasValue || WarrantyKm.HasValue;
    }

    public int? RemainingWarrantyDays(DateTimeOffset now)
    {
        if (!WarrantyExpiresAt.HasValue)
            return null;

        var remaining = (WarrantyExpiresAt.Value - now).TotalDays;
        return remaining > 0 ? (int)Math.Ceiling(remaining) : 0;
    }

    public int? RemainingWarrantyKm(int currentOdometer)
    {
        if (!WarrantyKm.HasValue)
            return null;

        var maxWarrantyKm = SoldOdometer + WarrantyKm.Value;
        var remaining = maxWarrantyKm - currentOdometer;
        return remaining > 0 ? remaining : 0;
    }

    public void MarkAsClaimed(Guid claimedByUserId, DateTimeOffset claimedAt)
    {
        if (claimedByUserId == Guid.Empty)
            throw new ArgumentException("ClaimedByUserId não pode ser vazio.", nameof(claimedByUserId));

        ClaimedByUserId = claimedByUserId;
        ClaimedAt = claimedAt;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
