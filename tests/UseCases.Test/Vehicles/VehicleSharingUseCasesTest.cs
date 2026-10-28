using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.RevokeVehicleShare;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.ShareVehicle;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.TransferVehicleOwnership;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetVehicleShares;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.Vehicles;

public class VehicleSharingUseCasesTest
{
    [Fact]
    public async Task ShareVehicle_Successfully_Shares_When_Owner()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var targetWorkspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId, userId: userId);

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdToUpdate(vehicle)
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(ownerWorkspaceId, userId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new ShareVehicleCommandHandler(vehicleWrite, workspaceProvider, unitOfWork);
        var command = new ShareVehicleCommand(vehicle.Id, new ShareVehicleRequestDto(targetWorkspaceId, VehicleWorkspaceRole.Contributor));

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(targetWorkspaceId, result.WorkspaceId);
        Assert.Equal(VehicleWorkspaceRole.Contributor, result.Role);
        Assert.True(vehicle.HasAccess(targetWorkspaceId, VehicleWorkspaceRole.Contributor));
    }

    [Fact]
    public async Task ShareVehicle_Throws_Forbidden_When_Not_Owner()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var nonOwnerWorkspaceId = Guid.NewGuid();
        var targetWorkspaceId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId);
        vehicle.AssignWorkspace(nonOwnerWorkspaceId, VehicleWorkspaceRole.Contributor, Guid.NewGuid());

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdToUpdate(vehicle)
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(nonOwnerWorkspaceId, Guid.NewGuid())
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new ShareVehicleCommandHandler(vehicleWrite, workspaceProvider, unitOfWork);
        var command = new ShareVehicleCommand(vehicle.Id, new ShareVehicleRequestDto(targetWorkspaceId, VehicleWorkspaceRole.Viewer));

        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task RevokeVehicleShare_Successfully_Revokes_When_Owner()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var targetWorkspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId, userId: userId);
        vehicle.AssignWorkspace(targetWorkspaceId, VehicleWorkspaceRole.Viewer, userId);

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdToUpdate(vehicle)
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(ownerWorkspaceId, userId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new RevokeVehicleShareCommandHandler(vehicleWrite, workspaceProvider, unitOfWork);
        var command = new RevokeVehicleShareCommand(vehicle.Id, targetWorkspaceId);

        await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.False(vehicle.HasAccess(targetWorkspaceId, VehicleWorkspaceRole.Viewer));
    }

    [Fact]
    public async Task RevokeVehicleShare_Throws_Forbidden_When_Not_Owner()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var contributorWorkspaceId = Guid.NewGuid();
        var targetWorkspaceId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId);
        vehicle.AssignWorkspace(contributorWorkspaceId, VehicleWorkspaceRole.Contributor, Guid.NewGuid());
        vehicle.AssignWorkspace(targetWorkspaceId, VehicleWorkspaceRole.Viewer, Guid.NewGuid());

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdToUpdate(vehicle)
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(contributorWorkspaceId, Guid.NewGuid())
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new RevokeVehicleShareCommandHandler(vehicleWrite, workspaceProvider, unitOfWork);
        var command = new RevokeVehicleShareCommand(vehicle.Id, targetWorkspaceId);

        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task TransferVehicleOwnership_Successfully_Transfers()
    {
        var currentOwnerWorkspaceId = Guid.NewGuid();
        var newOwnerWorkspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(workspaceId: currentOwnerWorkspaceId, userId: userId);
        vehicle.AssignWorkspace(newOwnerWorkspaceId, VehicleWorkspaceRole.Contributor, userId);

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdToUpdate(vehicle)
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(currentOwnerWorkspaceId, userId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new TransferVehicleOwnershipCommandHandler(vehicleWrite, workspaceProvider, unitOfWork);
        var command = new TransferVehicleOwnershipCommand(vehicle.Id, new TransferOwnershipRequestDto(newOwnerWorkspaceId));

        await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.Equal(VehicleWorkspaceRole.Owner, vehicle.GetRoleInWorkspace(newOwnerWorkspaceId));
        Assert.Equal(VehicleWorkspaceRole.Contributor, vehicle.GetRoleInWorkspace(currentOwnerWorkspaceId));
    }

    [Fact]
    public async Task TransferVehicleOwnership_Throws_When_Same_Workspace()
    {
        var currentOwnerWorkspaceId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: currentOwnerWorkspaceId);

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdToUpdate(vehicle)
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(currentOwnerWorkspaceId, Guid.NewGuid())
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new TransferVehicleOwnershipCommandHandler(vehicleWrite, workspaceProvider, unitOfWork);
        var command = new TransferVehicleOwnershipCommand(vehicle.Id, new TransferOwnershipRequestDto(currentOwnerWorkspaceId));

        await Assert.ThrowsAsync<ErrorOnValidationException>(() => handler.Handle(command, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetVehicleShares_Returns_Active_Shares_For_Member()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var memberWorkspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId, userId: userId);
        vehicle.AssignWorkspace(memberWorkspaceId, VehicleWorkspaceRole.Viewer, userId);

        var (vehicleRead, _) = new VehicleRepositoryBuilder()
            .SetupGetById(vehicle)
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(memberWorkspaceId, userId)
            .Build();

        var handler = new GetVehicleSharesQueryHandler(vehicleRead, workspaceProvider);
        var result = await handler.Handle(new GetVehicleSharesQuery(vehicle.Id), TestContext.Current.CancellationToken);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.WorkspaceId == ownerWorkspaceId && s.Role == VehicleWorkspaceRole.Owner);
        Assert.Contains(result, s => s.WorkspaceId == memberWorkspaceId && s.Role == VehicleWorkspaceRole.Viewer);
    }
}
