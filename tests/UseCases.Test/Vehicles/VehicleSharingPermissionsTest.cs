using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.CreateServiceOrder;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.UpdateOdometer;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.Vehicles;

public class VehicleSharingPermissionsTest
{
    [Fact]
    public async Task Viewer_Cannot_Update_Odometer_Throws_Forbidden()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var viewerWorkspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId, initialOdometer: 20_000);
        vehicle.AssignWorkspace(viewerWorkspaceId, VehicleWorkspaceRole.Viewer, userId);

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdate(vehicle)
            .Build();

        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder()
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(viewerWorkspaceId, userId)
            .Build();

        var handler = new UpdateOdometerCommandHandler(
            vehicleWrite,
            scheduleWrite,
            workspaceProvider,
            UnitOfWorkBuilder.Build());

        var command = new UpdateOdometerCommand(vehicle.Id, new UpdateOdometerRequestDto(21_000, DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Contributor_Can_Update_Odometer()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var contributorWorkspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId, initialOdometer: 20_000);
        vehicle.AssignWorkspace(contributorWorkspaceId, VehicleWorkspaceRole.Contributor, userId);

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdate(vehicle)
            .Build();

        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder()
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(contributorWorkspaceId, userId)
            .Build();

        var handler = new UpdateOdometerCommandHandler(
            vehicleWrite,
            scheduleWrite,
            workspaceProvider,
            UnitOfWorkBuilder.Build());

        var command = new UpdateOdometerCommand(vehicle.Id, new UpdateOdometerRequestDto(21_000, DateTimeOffset.UtcNow));

        await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.Equal(21_000, vehicle.CurrentOdometer);
    }

    [Fact]
    public async Task Viewer_Cannot_Create_Service_Order_Throws_Forbidden()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var viewerWorkspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId, initialOdometer: 20_000);
        vehicle.AssignWorkspace(viewerWorkspaceId, VehicleWorkspaceRole.Viewer, userId);

        var requestDto = CreateServiceOrderRequestBuilder.Build(odometerAtService: 22_000);

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdate(vehicle)
            .Build();

        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();
        var (_, soWrite) = new ServiceOrderRepositoryBuilder().Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(viewerWorkspaceId, userId)
            .Build();

        var handler = new CreateServiceOrderCommandHandler(
            vehicleWrite,
            scheduleWrite,
            soWrite,
            workspaceProvider,
            UnitOfWorkBuilder.Build());

        var command = new CreateServiceOrderCommand(vehicle.Id, requestDto);

        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Contributor_Can_Create_Service_Order()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var contributorWorkspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId, initialOdometer: 20_000);
        vehicle.AssignWorkspace(contributorWorkspaceId, VehicleWorkspaceRole.Contributor, userId);

        var requestDto = CreateServiceOrderRequestBuilder.Build(odometerAtService: 22_000);

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdate(vehicle)
            .Build();

        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();
        var (_, soWrite) = new ServiceOrderRepositoryBuilder().Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(contributorWorkspaceId, userId)
            .Build();

        var handler = new CreateServiceOrderCommandHandler(
            vehicleWrite,
            scheduleWrite,
            soWrite,
            workspaceProvider,
            UnitOfWorkBuilder.Build());

        var command = new CreateServiceOrderCommand(vehicle.Id, requestDto);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(vehicle.Id, result.VehicleId);
    }
}
