using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.Vehicles;

public class WorkspaceVehicle : BaseEntity
{
    public Guid WorkspaceId { get; private set; }
    public Guid VehicleId { get; private set; }
    public VehicleWorkspaceRole Role { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    public virtual Vehicle Vehicle { get; private init; } = default!;

    private WorkspaceVehicle() { }

    public static WorkspaceVehicle Create(
        Guid workspaceId,
        Guid vehicleId,
        VehicleWorkspaceRole role,
        Guid createdByUserId)
    {
        return new WorkspaceVehicle
        {
            WorkspaceId = workspaceId,
            VehicleId = vehicleId,
            Role = role,
            CreatedByUserId = createdByUserId
        };
    }

    public void UpdateRole(VehicleWorkspaceRole newRole)
    {
        Role = newRole;
        Touch();
    }
}
