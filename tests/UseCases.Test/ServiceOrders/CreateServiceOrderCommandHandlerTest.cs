using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.CreateServiceOrder;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.ServiceOrders;

public class CreateServiceOrderCommandHandlerTest
{
    [Fact]
    public async Task Creates_ServiceOrder_And_Updates_Odometer_And_Performs_Schedules()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: workspaceId, userId: userId, initialOdometer: 20_000);
        var schedule = MaintenanceScheduleBuilder.Build(
            workspaceId: workspaceId,
            userId: userId,
            vehicleId: vehicle.Id,
            currentOdometer: 20_000,
            intervalOdometer: 10_000);

        var requestDto = CreateServiceOrderRequestBuilder.Build(
            odometerAtService: 24_891,
            totalCost: 178.70m,
            scheduleId: schedule.Id);

        var (vehicleRead, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdate(vehicle)
            .Build();

        var (scheduleRead, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByVehicleIdToUpdate(new[] { schedule })
            .Build();

        var (soRead, soWrite) = new ServiceOrderRepositoryBuilder()
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(workspaceId, userId)
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
        Assert.Equal(24_891, result.OdometerAtService);
        Assert.Equal(178.70m, result.TotalCost);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(24_891, vehicle.CurrentOdometer);
        Assert.Equal(24_891, schedule.LastPerformedOdometer);
    }

    [Fact]
    public async Task Throws_NotFound_When_Vehicle_Does_Not_Exist()
    {
        var (vehicleRead, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdate(null)
            .Build();

        var (scheduleRead, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();
        var (soRead, soWrite) = new ServiceOrderRepositoryBuilder().Build();
        var workspaceProvider = new WorkspaceProviderBuilder().Build();

        var handler = new CreateServiceOrderCommandHandler(
            vehicleWrite,
            scheduleWrite,
            soWrite,
            workspaceProvider,
            UnitOfWorkBuilder.Build());

        var command = new CreateServiceOrderCommand(Guid.NewGuid(), CreateServiceOrderRequestBuilder.Build());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));
    }
}
