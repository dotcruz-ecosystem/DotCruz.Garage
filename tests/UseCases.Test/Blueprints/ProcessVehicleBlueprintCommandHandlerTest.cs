using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.DTOs.Ai;
using DotCruz.Garage.Application.UseCases.Blueprints.Commands.ProcessVehicleBlueprint;
using DotCruz.Garage.Domain.Enums;

namespace UseCases.Test.Blueprints;

public class ProcessVehicleBlueprintCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Processes_Pending_Blueprint_With_AiService()
    {
        var blueprint = VehicleBlueprintBuilder.Build(
            brand: "Honda",
            model: "Civic",
            engine: "2.0 Touring",
            type: VehicleType.Car,
            yearFrom: 2022,
            yearTo: 2026,
            withTemplates: false);

        var blueprintRepository = new VehicleBlueprintRepositoryBuilder()
            .WithBlueprint(blueprint)
            .Build();

        var aiService = new AiManualExtractionServiceBuilder().Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var logger = LoggerBuilder.Build<ProcessVehicleBlueprintCommandHandler>();

        var handler = new ProcessVehicleBlueprintCommandHandler(
            blueprintRepository,
            aiService,
            unitOfWork,
            logger);

        var command = new ProcessVehicleBlueprintCommand(blueprint.Id);

        await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.Equal(BlueprintStatus.Ready, blueprint.Status);
        Assert.NotEmpty(blueprint.Templates);
        Assert.Equal(2, blueprint.Templates.Count);
        Assert.NotNull(blueprint.SourceUrl);
    }

    [Fact]
    public async Task Gracefully_Fails_When_AiService_Throws_Exception()
    {
        var blueprint = VehicleBlueprintBuilder.Build(
            brand: "Yamaha",
            model: "MT-03",
            engine: "321cc",
            type: VehicleType.Motorcycle,
            withTemplates: false);

        var blueprintRepository = new VehicleBlueprintRepositoryBuilder()
            .WithBlueprint(blueprint)
            .Build();

        var aiServiceBuilder = new AiManualExtractionServiceBuilder();
        aiServiceBuilder.SetupThrows(new HttpRequestException("API timeout"));

        var unitOfWork = UnitOfWorkBuilder.Build();
        var logger = LoggerBuilder.Build<ProcessVehicleBlueprintCommandHandler>();

        var handler = new ProcessVehicleBlueprintCommandHandler(
            blueprintRepository,
            aiServiceBuilder.Build(),
            unitOfWork,
            logger);

        var command = new ProcessVehicleBlueprintCommand(blueprint.Id);

        await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.Equal(BlueprintStatus.Failed, blueprint.Status);
        Assert.Contains("API timeout", blueprint.ErrorMessage);
    }
}
