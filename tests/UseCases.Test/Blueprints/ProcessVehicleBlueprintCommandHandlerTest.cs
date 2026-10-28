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

        var (vehicleReadRepository, _) = new VehicleRepositoryBuilder().Build();
        var authServiceClient = new AuthServiceClientBuilder().Build();
        var notificationService = new NotificationServiceBuilder().Build();
        var aiService = new AiManualExtractionServiceBuilder().Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var logger = LoggerBuilder.Build<ProcessVehicleBlueprintCommandHandler>();

        var handler = new ProcessVehicleBlueprintCommandHandler(
            blueprintRepository,
            vehicleReadRepository,
            authServiceClient,
            notificationService,
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

        var (vehicleReadRepository, _) = new VehicleRepositoryBuilder().Build();
        var authServiceClient = new AuthServiceClientBuilder().Build();
        var notificationService = new NotificationServiceBuilder().Build();
        var aiServiceBuilder = new AiManualExtractionServiceBuilder();
        aiServiceBuilder.SetupThrows(new HttpRequestException("API timeout"));

        var unitOfWork = UnitOfWorkBuilder.Build();
        var logger = LoggerBuilder.Build<ProcessVehicleBlueprintCommandHandler>();

        var handler = new ProcessVehicleBlueprintCommandHandler(
            blueprintRepository,
            vehicleReadRepository,
            authServiceClient,
            notificationService,
            aiServiceBuilder.Build(),
            unitOfWork,
            logger);

        var command = new ProcessVehicleBlueprintCommand(blueprint.Id);

        await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.Equal(BlueprintStatus.Failed, blueprint.Status);
        Assert.Contains("API timeout", blueprint.ErrorMessage);
    }

    [Fact]
    public async Task Dispatches_Notifications_To_Owners_Of_Matching_Vehicles_When_Ready()
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

        var vehicle = VehicleBuilder.Build(
            brand: "Honda",
            model: "Civic",
            engine: "2.0 Touring");

        var vehicleRepoBuilder = new VehicleRepositoryBuilder()
            .SetupGetVehiclesByAttributes(new[] { vehicle });
        var (vehicleReadRepository, _) = vehicleRepoBuilder.Build();

        var user = new DotCruz.Garage.Application.Abstractions.Services.Auth.DTOs.UserResponseDto(
            vehicle.UserId,
            "Matheus Cruz",
            "matheus@dotcruz.com");

        var authServiceClient = new AuthServiceClientBuilder()
            .SetupGetUserById(vehicle.UserId, vehicle.Workspaces.First().WorkspaceId, user)
            .Build();

        var notificationServiceBuilder = new NotificationServiceBuilder();
        var aiService = new AiManualExtractionServiceBuilder().Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var logger = LoggerBuilder.Build<ProcessVehicleBlueprintCommandHandler>();

        var handler = new ProcessVehicleBlueprintCommandHandler(
            blueprintRepository,
            vehicleReadRepository,
            authServiceClient,
            notificationServiceBuilder.Build(),
            aiService,
            unitOfWork,
            logger);

        var command = new ProcessVehicleBlueprintCommand(blueprint.Id);

        await handler.Handle(command, TestContext.Current.CancellationToken);

        notificationServiceBuilder.VerifyBlueprintReadyNotificationSent(
            vehicle.UserId,
            vehicle.Id,
            Moq.Times.Once());
    }
}

