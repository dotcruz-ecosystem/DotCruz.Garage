using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Blueprints.Queries.GetVehicleBlueprintByVehicleId;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;

namespace UseCases.Test.Blueprints;

public class GetVehicleBlueprintByVehicleIdQueryHandlerTest
{
    [Fact]
    public async Task Returns_Blueprint_With_AlreadyScheduled_Flags()
    {
        var vehicle = VehicleBuilder.Build(
            brand: "Volkswagen",
            model: "Golf",
            engine: "1.4 TSI",
            type: VehicleType.Car);

        var blueprint = VehicleBlueprintBuilder.Build(
            brand: "Volkswagen",
            model: "Golf",
            engine: "1.4 TSI",
            type: VehicleType.Car,
            withTemplates: true);

        var firstTemplate = blueprint.Templates.First();

        var (vehicleRead, _) = new VehicleRepositoryBuilder()
            .SetupGetById(vehicle)
            .Build();

        var blueprintRepository = new VehicleBlueprintRepositoryBuilder()
            .WithBlueprint(blueprint)
            .Build();

        var existingSchedule = MaintenanceSchedule.Create(
            vehicle.Workspaces.First().WorkspaceId,
            vehicle.UserId,
            vehicle.Id,
            firstTemplate.Description,
            firstTemplate.Category,
            firstTemplate.IntervalOdometer,
            firstTemplate.IntervalTimeMonths,
            null,
            null,
            null,
            null,
            vehicle.CurrentOdometer,
            vehicle.DailyMileageAverage);

        var (scheduleRead, _) = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByVehicleId(new List<MaintenanceSchedule> { existingSchedule })
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new GetVehicleBlueprintByVehicleIdQueryHandler(
            vehicleRead,
            blueprintRepository,
            scheduleRead,
            unitOfWork);

        var query = new GetVehicleBlueprintByVehicleIdQuery(vehicle.Id);

        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(blueprint.Id, result.Id);
        Assert.Equal(BlueprintStatus.Ready, result.Status);

        var templateDto = result.Templates.First(t => t.Description == firstTemplate.Description);
        Assert.True(templateDto.AlreadyScheduled);

        var otherTemplate = result.Templates.First(t => t.Description != firstTemplate.Description);
        Assert.False(otherTemplate.AlreadyScheduled);
    }

    [Fact]
    public async Task Creates_Pending_Blueprint_When_None_Exists_Yet()
    {
        var vehicle = VehicleBuilder.Build(
            brand: "Ducati",
            model: "Panigale V4",
            engine: "1103cc",
            type: VehicleType.Motorcycle);

        var (vehicleRead, _) = new VehicleRepositoryBuilder()
            .SetupGetById(vehicle)
            .Build();

        var blueprintRepository = new VehicleBlueprintRepositoryBuilder().Build();

        var (scheduleRead, _) = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByVehicleId(new List<MaintenanceSchedule>())
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new GetVehicleBlueprintByVehicleIdQueryHandler(
            vehicleRead,
            blueprintRepository,
            scheduleRead,
            unitOfWork);

        var query = new GetVehicleBlueprintByVehicleIdQuery(vehicle.Id);

        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(BlueprintStatus.Pending, result.Status);
        Assert.Equal("Ducati", result.Brand);
        Assert.Equal("Panigale V4", result.Model);
        Assert.Equal("1103cc", result.Engine);
    }
}
