using CommonTestUtilities.Data;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;

namespace UseCases.Test.Vehicles;

public class CreateVehicleCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Creates_Vehicle_With_Explicit_Name()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        Vehicle? savedVehicle = null;

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupAdd(v => savedVehicle = v)
            .Build();

        var catalogRepository = new VehicleCatalogRepositoryBuilder().Build();
        var blueprintRepository = new VehicleBlueprintRepositoryBuilder().Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(workspaceId, userId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new CreateVehicleCommandHandler(
            vehicleWrite,
            catalogRepository,
            blueprintRepository,
            workspaceProvider,
            unitOfWork);

        var command = CreateVehicleCommandBuilder.Build(
            name: "Meu Corolla Executivo",
            type: VehicleType.Car,
            brand: "Toyota",
            model: "Corolla",
            engine: "2.0 Dynamic Force",
            year: 2024,
            plate: "bra2e19",
            initialOdometer: 15_000,
            initialDailyMileageAverage: 40);

        var vehicleId = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, vehicleId);
        Assert.NotNull(savedVehicle);
        Assert.Equal(vehicleId, savedVehicle.Id);
        Assert.Equal("Meu Corolla Executivo", savedVehicle.Name);
        Assert.Equal(VehicleType.Car, savedVehicle.Type);
        Assert.Equal("Toyota", savedVehicle.Brand);
        Assert.Equal("Corolla", savedVehicle.Model);
        Assert.Equal("2.0 Dynamic Force", savedVehicle.Engine);
        Assert.Equal(2024, savedVehicle.Year);
        Assert.Equal("BRA2E19", savedVehicle.Plate);
        Assert.Equal(15_000, savedVehicle.CurrentOdometer);
        Assert.Single(savedVehicle.OdometerHistories);
        Assert.Equal(15_000, savedVehicle.OdometerHistories.First().OdometerValue);
        Assert.Single(savedVehicle.Workspaces);
        Assert.Equal(workspaceId, savedVehicle.Workspaces.First().WorkspaceId);
        Assert.Equal(VehicleWorkspaceRole.Owner, savedVehicle.Workspaces.First().Role);
    }

    [Fact]
    public async Task Successfully_Creates_Vehicle_With_Default_Name_When_Name_Is_Null_Or_Whitespace()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        Vehicle? savedVehicle = null;

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupAdd(v => savedVehicle = v)
            .Build();

        var catalogRepository = new VehicleCatalogRepositoryBuilder().Build();
        var blueprintRepository = new VehicleBlueprintRepositoryBuilder().Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(workspaceId, userId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new CreateVehicleCommandHandler(
            vehicleWrite,
            catalogRepository,
            blueprintRepository,
            workspaceProvider,
            unitOfWork);

        var command = CreateVehicleCommandBuilder.Build(
            name: null,
            type: VehicleType.Motorcycle,
            brand: "Honda",
            model: "CB 300F",
            engine: "300cc",
            year: 2023,
            withoutPlate: true);

        var vehicleId = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, vehicleId);
        Assert.NotNull(savedVehicle);
        Assert.Equal("CB 300F 300cc - Honda - 2023", savedVehicle.Name);
        Assert.Equal(VehicleType.Motorcycle, savedVehicle.Type);
        Assert.Equal("Honda", savedVehicle.Brand);
        Assert.Equal("CB 300F", savedVehicle.Model);
        Assert.Equal("300cc", savedVehicle.Engine);
        Assert.Null(savedVehicle.Plate);
    }
}
