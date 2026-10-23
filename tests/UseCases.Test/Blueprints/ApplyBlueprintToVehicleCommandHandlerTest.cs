using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.DTOs.Blueprints;
using DotCruz.Garage.Application.UseCases.Blueprints.Commands.ApplyBlueprintToVehicle;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.Blueprints;

public class ApplyBlueprintToVehicleCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Applies_NeverPerformed_Template_Calculating_From_Zero_Odometer()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(
            workspaceId: workspaceId,
            userId: userId,
            brand: "Toyota",
            model: "Corolla",
            engine: "2.0 Flex",
            type: VehicleType.Car,
            initialOdometer: 50_000);

        var blueprint = VehicleBlueprintBuilder.Build(
            brand: "Toyota",
            model: "Corolla",
            engine: "2.0 Flex",
            type: VehicleType.Car,
            withTemplates: true);

        var template = blueprint.Templates.First();

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdate(vehicle)
            .Build();

        var blueprintRepository = new VehicleBlueprintRepositoryBuilder()
            .WithBlueprint(blueprint)
            .Build();

        var addedSchedules = new List<MaintenanceSchedule>();
        var scheduleRepoBuilder = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByVehicleId(new List<MaintenanceSchedule>())
            .SetupAdd(s => addedSchedules.Add(s));
        var (scheduleRead, scheduleWrite) = scheduleRepoBuilder.Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(workspaceId, userId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new ApplyBlueprintToVehicleCommandHandler(
            vehicleWrite,
            blueprintRepository,
            scheduleRead,
            scheduleWrite,
            workspaceProvider,
            unitOfWork);

        var command = new ApplyBlueprintToVehicleCommand(
            vehicle.Id,
            new ApplyBlueprintRequestDto(new List<ApplyBlueprintItemDto>
            {
                new(template.Id, "NeverPerformed", null, null)
            }));

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotEmpty(result);
        Assert.Single(addedSchedules);

        var created = addedSchedules.First();
        Assert.Equal(template.Description, created.Description);
        Assert.Equal(0, created.LastPerformedOdometer);
        // O próximo vencimento foi calculado em 10.000 km, com carro a 50.000 km deve estar Overdue
        Assert.Equal(ScheduleStatus.Overdue, created.Status);
    }

    [Fact]
    public async Task Successfully_Applies_RecentlyPerformed_Template_With_User_Provided_Odometer()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(
            workspaceId: workspaceId,
            userId: userId,
            brand: "Honda",
            model: "Civic",
            engine: "2.0",
            type: VehicleType.Car,
            initialOdometer: 50_000);

        var blueprint = VehicleBlueprintBuilder.Build(
            brand: "Honda",
            model: "Civic",
            engine: "2.0",
            type: VehicleType.Car,
            withTemplates: true);

        var template = blueprint.Templates.First(); // Troca de Óleo: 10_000 km

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdate(vehicle)
            .Build();

        var blueprintRepository = new VehicleBlueprintRepositoryBuilder()
            .WithBlueprint(blueprint)
            .Build();

        var addedSchedules = new List<MaintenanceSchedule>();
        var scheduleRepoBuilder = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByVehicleId(new List<MaintenanceSchedule>())
            .SetupAdd(s => addedSchedules.Add(s));
        var (scheduleRead, scheduleWrite) = scheduleRepoBuilder.Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(workspaceId, userId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new ApplyBlueprintToVehicleCommandHandler(
            vehicleWrite,
            blueprintRepository,
            scheduleRead,
            scheduleWrite,
            workspaceProvider,
            unitOfWork);

        var command = new ApplyBlueprintToVehicleCommand(
            vehicle.Id,
            new ApplyBlueprintRequestDto(new List<ApplyBlueprintItemDto>
            {
                new(template.Id, "RecentlyPerformed", 45_000, DateTimeOffset.UtcNow.AddDays(-10))
            }));

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotEmpty(result);
        Assert.Single(addedSchedules);

        var created = addedSchedules.First();
        Assert.Equal(45_000, created.LastPerformedOdometer);
        Assert.Equal(55_000, created.NextDueOdometer); // 45k + 10k
        Assert.Equal(ScheduleStatus.Active, created.Status);
    }

    [Fact]
    public async Task Successfully_Applies_PendingFirstExecution_Template()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(
            workspaceId: workspaceId,
            userId: userId,
            brand: "Honda",
            model: "Civic",
            engine: "2.0",
            type: VehicleType.Car);

        var blueprint = VehicleBlueprintBuilder.Build(
            brand: "Honda",
            model: "Civic",
            engine: "2.0",
            type: VehicleType.Car,
            withTemplates: true);

        var template = blueprint.Templates.First();

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdate(vehicle)
            .Build();

        var blueprintRepository = new VehicleBlueprintRepositoryBuilder()
            .WithBlueprint(blueprint)
            .Build();

        var addedSchedules = new List<MaintenanceSchedule>();
        var scheduleRepoBuilder = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByVehicleId(new List<MaintenanceSchedule>())
            .SetupAdd(s => addedSchedules.Add(s));
        var (scheduleRead, scheduleWrite) = scheduleRepoBuilder.Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(workspaceId, userId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new ApplyBlueprintToVehicleCommandHandler(
            vehicleWrite,
            blueprintRepository,
            scheduleRead,
            scheduleWrite,
            workspaceProvider,
            unitOfWork);

        var command = new ApplyBlueprintToVehicleCommand(
            vehicle.Id,
            new ApplyBlueprintRequestDto(new List<ApplyBlueprintItemDto>
            {
                new(template.Id, "PendingFirstExecution", null, null)
            }));

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotEmpty(result);
        Assert.Single(addedSchedules);

        var created = addedSchedules.First();
        Assert.Null(created.LastPerformedOdometer);
        Assert.Null(created.NextDueOdometer);
        Assert.Equal(ScheduleStatus.PendingFirstExecution, created.Status);
    }

    [Fact]
    public async Task Throws_ErrorOnValidationException_When_Blueprint_Not_Ready()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(
            workspaceId: workspaceId,
            userId: userId,
            brand: "Yamaha",
            model: "MT-07",
            engine: "689cc",
            type: VehicleType.Motorcycle);

        var blueprint = VehicleBlueprintBuilder.Build(
            brand: "Yamaha",
            model: "MT-07",
            engine: "689cc",
            type: VehicleType.Motorcycle,
            withTemplates: false); // Status = Pending

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdate(vehicle)
            .Build();

        var blueprintRepository = new VehicleBlueprintRepositoryBuilder()
            .WithBlueprint(blueprint)
            .Build();

        var (scheduleRead, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(workspaceId, userId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new ApplyBlueprintToVehicleCommandHandler(
            vehicleWrite,
            blueprintRepository,
            scheduleRead,
            scheduleWrite,
            workspaceProvider,
            unitOfWork);

        var command = new ApplyBlueprintToVehicleCommand(
            vehicle.Id,
            new ApplyBlueprintRequestDto(new List<ApplyBlueprintItemDto>
            {
                new(Guid.NewGuid(), "NeverPerformed", null, null)
            }));

        await Assert.ThrowsAsync<ErrorOnValidationException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));
    }
}
