using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Entities.Vehicles;

namespace DotCruz.Garage.Domain.Entities.Drivers;

public class VehicleDriverAssignment : BaseEntity, IWorkspaceEntity
{
    public Guid WorkspaceId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid DriverId { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? EndedAt { get; private set; }
    public string? Notes { get; private set; }

    public virtual Vehicle Vehicle { get; private init; } = default!;
    public virtual Driver Driver { get; private init; } = default!;

    public bool IsCurrent => EndedAt == null;

    private VehicleDriverAssignment() { }

    public static VehicleDriverAssignment Create(
        Guid workspaceId,
        Guid userId,
        Guid vehicleId,
        Guid driverId,
        DateTimeOffset startedAt,
        string? notes = null)
    {
        return new VehicleDriverAssignment
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            VehicleId = vehicleId,
            DriverId = driverId,
            StartedAt = startedAt,
            Notes = notes
        };
    }

    public void EndAssignment(DateTimeOffset? endedAt = null)
    {
        EndedAt = endedAt ?? DateTimeOffset.UtcNow;
    }
}
