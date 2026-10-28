using CommonTestUtilities.Entities;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;

namespace UseCases.Test.Vehicles;

public class VehicleWorkspaceSharingDomainTest
{
    [Fact]
    public void Vehicle_AssignWorkspace_Adds_New_Link_With_Given_Role()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId);

        var familyWorkspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var link = vehicle.AssignWorkspace(familyWorkspaceId, VehicleWorkspaceRole.Contributor, userId);

        Assert.Equal(familyWorkspaceId, link.WorkspaceId);
        Assert.Equal(VehicleWorkspaceRole.Contributor, link.Role);
        Assert.Equal(userId, link.CreatedByUserId);
        Assert.Equal(2, vehicle.Workspaces.Count);
    }

    [Fact]
    public void Vehicle_AssignWorkspace_Updates_Role_If_Already_Exists()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId);

        var familyWorkspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        vehicle.AssignWorkspace(familyWorkspaceId, VehicleWorkspaceRole.Viewer, userId);
        var updated = vehicle.AssignWorkspace(familyWorkspaceId, VehicleWorkspaceRole.Contributor, userId);

        Assert.Equal(VehicleWorkspaceRole.Contributor, updated.Role);
        Assert.Equal(2, vehicle.Workspaces.Count);
    }

    [Fact]
    public void Vehicle_RemoveWorkspace_Soft_Deletes_Link()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId);

        var familyWorkspaceId = Guid.NewGuid();
        vehicle.AssignWorkspace(familyWorkspaceId, VehicleWorkspaceRole.Viewer, Guid.NewGuid());

        var removed = vehicle.RemoveWorkspace(familyWorkspaceId);

        Assert.True(removed);
        Assert.False(vehicle.HasAccess(familyWorkspaceId, VehicleWorkspaceRole.Viewer));
    }

    [Fact]
    public void Vehicle_RemoveWorkspace_Returns_False_If_Not_Found()
    {
        var vehicle = VehicleBuilder.Build();
        var removed = vehicle.RemoveWorkspace(Guid.NewGuid());
        Assert.False(removed);
    }

    [Fact]
    public void Vehicle_TransferOwnership_Updates_Previous_Owner_To_Contributor_And_New_To_Owner()
    {
        var oldOwnerWorkspaceId = Guid.NewGuid();
        var newOwnerWorkspaceId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: oldOwnerWorkspaceId);

        vehicle.AssignWorkspace(newOwnerWorkspaceId, VehicleWorkspaceRole.Contributor, Guid.NewGuid());

        vehicle.TransferOwnership(newOwnerWorkspaceId);

        Assert.Equal(VehicleWorkspaceRole.Contributor, vehicle.GetRoleInWorkspace(oldOwnerWorkspaceId));
        Assert.Equal(VehicleWorkspaceRole.Owner, vehicle.GetRoleInWorkspace(newOwnerWorkspaceId));
        Assert.True(vehicle.IsOwner(newOwnerWorkspaceId));
        Assert.False(vehicle.IsOwner(oldOwnerWorkspaceId));
    }

    [Fact]
    public void Vehicle_HasAccess_Enforces_Role_Hierarchy()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var contributorWorkspaceId = Guid.NewGuid();
        var viewerWorkspaceId = Guid.NewGuid();
        var outsiderWorkspaceId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId);
        vehicle.AssignWorkspace(contributorWorkspaceId, VehicleWorkspaceRole.Contributor, Guid.NewGuid());
        vehicle.AssignWorkspace(viewerWorkspaceId, VehicleWorkspaceRole.Viewer, Guid.NewGuid());

        // Owner access checks
        Assert.True(vehicle.HasAccess(ownerWorkspaceId, VehicleWorkspaceRole.Owner));
        Assert.True(vehicle.HasAccess(ownerWorkspaceId, VehicleWorkspaceRole.Contributor));
        Assert.True(vehicle.HasAccess(ownerWorkspaceId, VehicleWorkspaceRole.Viewer));

        // Contributor access checks
        Assert.False(vehicle.HasAccess(contributorWorkspaceId, VehicleWorkspaceRole.Owner));
        Assert.True(vehicle.HasAccess(contributorWorkspaceId, VehicleWorkspaceRole.Contributor));
        Assert.True(vehicle.HasAccess(contributorWorkspaceId, VehicleWorkspaceRole.Viewer));

        // Viewer access checks
        Assert.False(vehicle.HasAccess(viewerWorkspaceId, VehicleWorkspaceRole.Owner));
        Assert.False(vehicle.HasAccess(viewerWorkspaceId, VehicleWorkspaceRole.Contributor));
        Assert.True(vehicle.HasAccess(viewerWorkspaceId, VehicleWorkspaceRole.Viewer));

        // Outsider access checks
        Assert.False(vehicle.HasAccess(outsiderWorkspaceId, VehicleWorkspaceRole.Viewer));
    }

    [Fact]
    public void Vehicle_CanWrite_And_IsOwner_Convenience_Methods()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var contributorWorkspaceId = Guid.NewGuid();
        var viewerWorkspaceId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId);
        vehicle.AssignWorkspace(contributorWorkspaceId, VehicleWorkspaceRole.Contributor, Guid.NewGuid());
        vehicle.AssignWorkspace(viewerWorkspaceId, VehicleWorkspaceRole.Viewer, Guid.NewGuid());

        Assert.True(vehicle.CanWrite(ownerWorkspaceId));
        Assert.True(vehicle.CanWrite(contributorWorkspaceId));
        Assert.False(vehicle.CanWrite(viewerWorkspaceId));

        Assert.True(vehicle.IsOwner(ownerWorkspaceId));
        Assert.False(vehicle.IsOwner(contributorWorkspaceId));
        Assert.False(vehicle.IsOwner(viewerWorkspaceId));
    }
}
