using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.UseCases.Blueprints.Commands.ProcessPendingBlueprints;
using DotCruz.Garage.Domain.Entities.Vehicles;

namespace UseCases.Test.Blueprints;

public class ProcessPendingBlueprintsCommandHandlerTest
{
    [Fact]
    public async Task Processes_Pending_Blueprints_Successfully()
    {
        var blueprint1 = VehicleBlueprintBuilder.Build(brand: "Fiat", model: "Pulse", engine: "1.0 Turbo", withTemplates: false);
        var blueprint2 = VehicleBlueprintBuilder.Build(brand: "Renault", model: "Kardian", engine: "1.0 Turbo", withTemplates: false);

        var blueprintRepository = new VehicleBlueprintRepositoryBuilder()
            .SetupGetPendingBlueprints(new List<VehicleBlueprint> { blueprint1, blueprint2 })
            .Build();

        var mediator = new MediatorBuilder().Build();
        var logger = LoggerBuilder.Build<ProcessPendingBlueprintsCommandHandler>();

        var handler = new ProcessPendingBlueprintsCommandHandler(blueprintRepository, mediator, logger);

        var result = await handler.Handle(new ProcessPendingBlueprintsCommand(Limit: 5), TestContext.Current.CancellationToken);

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task Returns_Zero_When_No_Pending_Blueprints()
    {
        var blueprintRepository = new VehicleBlueprintRepositoryBuilder()
            .SetupGetPendingBlueprints(new List<VehicleBlueprint>())
            .Build();

        var mediator = new MediatorBuilder().Build();
        var logger = LoggerBuilder.Build<ProcessPendingBlueprintsCommandHandler>();

        var handler = new ProcessPendingBlueprintsCommandHandler(blueprintRepository, mediator, logger);

        var result = await handler.Handle(new ProcessPendingBlueprintsCommand(Limit: 5), TestContext.Current.CancellationToken);

        Assert.Equal(0, result);
    }
}
