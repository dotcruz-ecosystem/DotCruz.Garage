namespace DotCruz.Garage.Domain.ValueObjects;

public sealed record PostSalePortalToken
{
    public Guid VehicleId { get; init; }
    public Guid SaleId { get; init; }
    public Guid WorkspaceId { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }

    public PostSalePortalToken(Guid vehicleId, Guid saleId, Guid workspaceId, DateTimeOffset expiresAt)
    {
        if (vehicleId == Guid.Empty)
            throw new ArgumentException("VehicleId não pode ser vazio.", nameof(vehicleId));

        if (saleId == Guid.Empty)
            throw new ArgumentException("SaleId não pode ser vazio.", nameof(saleId));

        if (workspaceId == Guid.Empty)
            throw new ArgumentException("WorkspaceId não pode ser vazio.", nameof(workspaceId));

        VehicleId = vehicleId;
        SaleId = saleId;
        WorkspaceId = workspaceId;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired(DateTimeOffset? now = null)
    {
        var reference = now ?? DateTimeOffset.UtcNow;
        return reference > ExpiresAt;
    }
}
