using CommonTestUtilities.Data;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;
using DotCruz.Garage.Domain.Entities.Vehicles;

namespace UseCases.Test.Vehicles;

public class CreateVehicleCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Creates_Vehicle_With_Explicit_Name()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        Vehicle? savedVehicle = null;

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupAdd(v => savedVehicle = v)
            .Build();

        var tenantProvider = new TenantProviderBuilder()
            .AsUser(tenantId, userId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new CreateVehicleCommandHandler(
            vehicleWrite,
            tenantProvider,
            unitOfWork);

        var command = CreateVehicleCommandBuilder.Build(
            name: "Meu Corolla Executivo",
            brand: "Toyota",
            model: "Corolla",
            year: 2024,
            plate: "bra2e19",
            vin: "9br12345678901234",
            initialOdometer: 15_000,
            initialDailyMileageAverage: 40);

        var vehicleId = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, vehicleId);
        Assert.NotNull(savedVehicle);
        Assert.Equal(vehicleId, savedVehicle.Id);
        Assert.Equal("Meu Corolla Executivo", savedVehicle.Name);
        Assert.Equal("Toyota", savedVehicle.Brand);
        Assert.Equal("Corolla", savedVehicle.Model);
        Assert.Equal(2024, savedVehicle.Year);
        Assert.Equal("BRA2E19", savedVehicle.Plate);
        Assert.Equal("9BR12345678901234", savedVehicle.Vin);
        Assert.Equal(15_000, savedVehicle.CurrentOdometer);
        Assert.Equal(40, savedVehicle.DailyMileageAverage);
        Assert.Single(savedVehicle.OdometerHistories);
        Assert.Equal(15_000, savedVehicle.OdometerHistories.First().OdometerValue);
    }

    [Fact]
    public async Task Successfully_Creates_Vehicle_With_Default_Name_When_Name_Is_Null_Or_Whitespace()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        Vehicle? savedVehicle = null;

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupAdd(v => savedVehicle = v)
            .Build();

        var tenantProvider = new TenantProviderBuilder()
            .AsUser(tenantId, userId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new CreateVehicleCommandHandler(
            vehicleWrite,
            tenantProvider,
            unitOfWork);

        var command = CreateVehicleCommandBuilder.Build(
            name: null,
            brand: "Fiat",
            model: "Strada",
            year: 2023,
            withoutPlate: true,
            withoutVin: true);

        var vehicleId = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, vehicleId);
        Assert.NotNull(savedVehicle);
        Assert.Equal("Strada - Fiat - 2023", savedVehicle.Name);
        Assert.Equal("Fiat", savedVehicle.Brand);
        Assert.Equal("Strada", savedVehicle.Model);
        Assert.Null(savedVehicle.Plate);
        Assert.Null(savedVehicle.Vin);
    }
}
