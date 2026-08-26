using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Catalog.Commands.ApplyBlueprintToVehicle;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using Moq;

namespace UseCases.Test.Catalog;

public class ApplyBlueprintToVehicleCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Applies_Blueprint_Templates_To_Vehicle()
    {
        var vehicle = VehicleBuilder.Build(initialOdometer: 20_000);
        var blueprint = VehicleBlueprintBuilder.Build(templatesCount: 4);

        var vehicleRepo = new VehicleRepositoryBuilder().SetupGetById(vehicle);
        var (vehicleRead, _) = vehicleRepo.Build();

        var blueprintRepo = new VehicleBlueprintRepositoryBuilder().SetupGetById(blueprint);
        var (blueprintRead, _) = blueprintRepo.Build();

        var scheduleRepo = new MaintenanceScheduleRepositoryBuilder();
        var (_, scheduleWrite) = scheduleRepo.Build();

        var handler = new ApplyBlueprintToVehicleCommandHandler(
            vehicleRead,
            blueprintRead,
            scheduleWrite,
            UnitOfWorkBuilder.Build());

        var result = await handler.Handle(new ApplyBlueprintToVehicleCommand(vehicle.Id, blueprint.Id), TestContext.Current.CancellationToken);

        Assert.Equal(4, result);
        scheduleRepo.WriteMock.Verify(s => s.AddAsync(It.IsAny<MaintenanceSchedule>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
    }

    [Fact]
    public async Task Throws_NotFoundException_When_Vehicle_Does_Not_Exist()
    {
        var (vehicleRead, _) = new VehicleRepositoryBuilder().SetupGetById(null).Build();
        var (blueprintRead, _) = new VehicleBlueprintRepositoryBuilder().Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();

        var handler = new ApplyBlueprintToVehicleCommandHandler(
            vehicleRead,
            blueprintRead,
            scheduleWrite,
            UnitOfWorkBuilder.Build());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new ApplyBlueprintToVehicleCommand(Guid.NewGuid(), Guid.NewGuid()), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Throws_NotFoundException_When_Blueprint_Does_Not_Exist()
    {
        var vehicle = VehicleBuilder.Build();
        var (vehicleRead, _) = new VehicleRepositoryBuilder().SetupGetById(vehicle).Build();
        var (blueprintRead, _) = new VehicleBlueprintRepositoryBuilder().SetupGetById(null).Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();

        var handler = new ApplyBlueprintToVehicleCommandHandler(
            vehicleRead,
            blueprintRead,
            scheduleWrite,
            UnitOfWorkBuilder.Build());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new ApplyBlueprintToVehicleCommand(vehicle.Id, Guid.NewGuid()), TestContext.Current.CancellationToken));
    }
}
