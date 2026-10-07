namespace DotCruz.Garage.Domain.ValueObjects;

public sealed record OdometerCheckinToken
{
    public Guid VehicleId { get; }
    public Guid WorkspaceId { get; }
    public DateTimeOffset ExpiresAt { get; }

    public OdometerCheckinToken(Guid vehicleId, Guid workspaceId, DateTimeOffset expiresAt)
    {
        VehicleId = vehicleId;
        WorkspaceId = workspaceId;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired(DateTimeOffset? now = null) =>
        (now ?? DateTimeOffset.UtcNow) > ExpiresAt;
}
