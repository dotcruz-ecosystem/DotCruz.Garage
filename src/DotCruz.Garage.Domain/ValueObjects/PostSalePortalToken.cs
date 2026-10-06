namespace DotCruz.Garage.Domain.ValueObjects;

public sealed record PostSalePortalToken
{
    public Guid VehicleId { get; init; }
    public Guid SaleId { get; init; }
    public Guid TenantId { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }

    public PostSalePortalToken(Guid vehicleId, Guid saleId, Guid tenantId, DateTimeOffset expiresAt)
    {
        if (vehicleId == Guid.Empty)
            throw new ArgumentException("VehicleId não pode ser vazio.", nameof(vehicleId));

        if (saleId == Guid.Empty)
            throw new ArgumentException("SaleId não pode ser vazio.", nameof(saleId));

        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId não pode ser vazio.", nameof(tenantId));

        VehicleId = vehicleId;
        SaleId = saleId;
        TenantId = tenantId;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired(DateTimeOffset? now = null)
    {
        var reference = now ?? DateTimeOffset.UtcNow;
        return reference > ExpiresAt;
    }
}
