using System.Text;
using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.UseCases.Catalog.Commands.ExtractManualMaintenance;
using DotCruz.Garage.Domain.Entities.Catalog;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using Moq;

namespace UseCases.Test.Catalog;

public class ExtractManualMaintenanceCommandHandlerTest
{
    [Fact]
    public async Task Returns_Existing_Blueprint_Without_Calling_AI_When_Found_In_Catalog()
    {
        var existingBlueprint = VehicleBlueprintBuilder.Build(
            brand: "Toyota",
            model: "Corolla",
            year: 2024,
            templatesCount: 3);

        var blueprintRepo = new VehicleBlueprintRepositoryBuilder()
            .SetupGetByDetails(existingBlueprint);

        var (blueprintRead, blueprintWrite) = blueprintRepo.Build();
        var (vehicleRead, _) = new VehicleRepositoryBuilder().Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();
        var aiParser = new AiManualParserServiceBuilder();

        var handler = new ExtractManualMaintenanceCommandHandler(
            blueprintRead,
            blueprintWrite,
            vehicleRead,
            scheduleWrite,
            aiParser.Build(),
            UnitOfWorkBuilder.Build());

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("fake pdf content"));
        var command = new ExtractManualMaintenanceCommand(
            FileStream: memoryStream,
            FileName: "manual_corolla.pdf",
            Brand: "Toyota",
            Model: "Corolla",
            Year: 2024,
            Engine: "2.0 Dynamic Force",
            Version: "XEi");

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(existingBlueprint.Id, result.Id);
        Assert.Equal("Toyota", result.Brand);
        Assert.Equal("Corolla", result.Model);
        Assert.Equal(3, result.PlanTemplates.Count);

        aiParser.Mock.Verify(a => a.ExtractMaintenancePlanAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);

        blueprintRepo.WriteMock.Verify(w => w.AddAsync(It.IsAny<VehicleBlueprint>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Calls_AI_And_Saves_New_Blueprint_When_Not_Found_In_Catalog()
    {
        var blueprintRepo = new VehicleBlueprintRepositoryBuilder()
            .SetupGetByDetails(null);

        var (blueprintRead, blueprintWrite) = blueprintRepo.Build();
        var (vehicleRead, _) = new VehicleRepositoryBuilder().Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();
        var aiParser = new AiManualParserServiceBuilder()
            .SetupDefaultExtraction(brand: "Fiat", model: "Strada", year: 2023);

        var handler = new ExtractManualMaintenanceCommandHandler(
            blueprintRead,
            blueprintWrite,
            vehicleRead,
            scheduleWrite,
            aiParser.Build(),
            UnitOfWorkBuilder.Build());

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("fake pdf content"));
        var command = new ExtractManualMaintenanceCommand(
            FileStream: memoryStream,
            FileName: "manual_strada.pdf",
            Brand: "Fiat",
            Model: "Strada",
            Year: 2023,
            Engine: "1.3",
            Version: "Endurance");

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal("Fiat", result.Brand);
        Assert.Equal("Strada", result.Model);
        Assert.Equal(2023, result.Year);
        Assert.Equal(2, result.PlanTemplates.Count);

        aiParser.Mock.Verify(a => a.ExtractMaintenancePlanAsync(
            It.IsAny<Stream>(), "manual_strada.pdf", "Fiat", "Strada", 2023, "1.3", It.IsAny<CancellationToken>()),
            Times.Once);

        blueprintRepo.WriteMock.Verify(w => w.AddAsync(It.IsAny<VehicleBlueprint>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Populates_Vehicle_Schedules_When_VehicleId_Is_Provided()
    {
        var vehicle = VehicleBuilder.Build(initialOdometer: 15_000);

        var blueprintRepo = new VehicleBlueprintRepositoryBuilder()
            .SetupGetByDetails(null);

        var (blueprintRead, blueprintWrite) = blueprintRepo.Build();
        var vehicleRepo = new VehicleRepositoryBuilder().SetupGetById(vehicle);
        var (vehicleRead, _) = vehicleRepo.Build();
        var scheduleRepo = new MaintenanceScheduleRepositoryBuilder();
        var (_, scheduleWrite) = scheduleRepo.Build();
        var aiParser = new AiManualParserServiceBuilder().SetupDefaultExtraction();

        var handler = new ExtractManualMaintenanceCommandHandler(
            blueprintRead,
            blueprintWrite,
            vehicleRead,
            scheduleWrite,
            aiParser.Build(),
            UnitOfWorkBuilder.Build());

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("fake pdf content"));
        var command = new ExtractManualMaintenanceCommand(
            FileStream: memoryStream,
            FileName: "manual.pdf",
            Brand: "Fiat",
            Model: "Strada",
            Year: 2023,
            Engine: "1.3",
            Version: "Endurance",
            VehicleId: vehicle.Id);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        scheduleRepo.WriteMock.Verify(s => s.AddAsync(It.IsAny<MaintenanceSchedule>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Throws_NotFoundException_When_VehicleId_Does_Not_Exist()
    {
        var blueprint = VehicleBlueprintBuilder.Build();
        var blueprintRepo = new VehicleBlueprintRepositoryBuilder().SetupGetByDetails(blueprint);
        var (blueprintRead, blueprintWrite) = blueprintRepo.Build();

        var vehicleRepo = new VehicleRepositoryBuilder().SetupGetById(null);
        var (vehicleRead, _) = vehicleRepo.Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();
        var aiParser = new AiManualParserServiceBuilder();

        var handler = new ExtractManualMaintenanceCommandHandler(
            blueprintRead,
            blueprintWrite,
            vehicleRead,
            scheduleWrite,
            aiParser.Build(),
            UnitOfWorkBuilder.Build());

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("fake pdf content"));
        var command = new ExtractManualMaintenanceCommand(
            FileStream: memoryStream,
            FileName: "manual.pdf",
            Brand: "Fiat",
            Model: "Strada",
            Year: 2023,
            Engine: "1.3",
            Version: "Endurance",
            VehicleId: Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));
    }
}
