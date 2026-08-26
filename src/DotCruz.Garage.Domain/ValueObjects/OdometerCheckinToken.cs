namespace DotCruz.Garage.Domain.ValueObjects;

public sealed record OdometerCheckinToken
{
    public Guid VehicleId { get; }
    public Guid TenantId { get; }
    public DateTimeOffset ExpiresAt { get; }

    public OdometerCheckinToken(Guid vehicleId, Guid tenantId, DateTimeOffset expiresAt)
    {
        VehicleId = vehicleId;
        TenantId = tenantId;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired(DateTimeOffset? now = null) =>
        (now ?? DateTimeOffset.UtcNow) > ExpiresAt;
}
