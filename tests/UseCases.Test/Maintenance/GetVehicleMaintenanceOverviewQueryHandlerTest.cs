using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetVehicleMaintenanceOverview;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;

namespace UseCases.Test.Maintenance;

public class GetVehicleMaintenanceOverviewQueryHandlerTest
{
    [Fact]
    public async Task Returns_Accurate_Overview_Statistics()
    {
        var tenantId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(tenantId: tenantId, initialOdometer: 50_000);

        var scheduleActive = MaintenanceScheduleBuilder.Build(
            tenantId: tenantId,
            vehicleId: vehicle.Id,
            currentOdometer: 50_000,
            lastPerformedOdometer: 50_000,
            intervalOdometer: 10_000,
            warningThresholdOdometer: 1_000);
        var scheduleDueSoon = MaintenanceSchedule.Create(
            tenantId, Guid.NewGuid(), vehicle.Id, "Revisão", MaintenanceCategory.General,
            10_000, null, 50_000, null, 1_000, null, 59_500, 20);

        var log1 = MaintenanceLogBuilder.Build(tenantId: tenantId, vehicleId: vehicle.Id, cost: 500m, performedAt: DateTimeOffset.UtcNow.AddMonths(-2));
        var log2 = MaintenanceLogBuilder.Build(tenantId: tenantId, vehicleId: vehicle.Id, cost: 300m, performedAt: DateTimeOffset.UtcNow.AddMonths(-1));

        var (vehicleRead, _) = new VehicleRepositoryBuilder().SetupGetById(vehicle).Build();
        var (scheduleRead, _) = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByVehicleId(new[] { scheduleActive, scheduleDueSoon })
            .Build();
        var (logRead, _) = new MaintenanceLogRepositoryBuilder()
            .SetupGetByVehicleIdPaginated(new[] { log1, log2 }, 2)
            .Build();

        var handler = new GetVehicleMaintenanceOverviewQueryHandler(
            vehicleRead,
            scheduleRead,
            logRead);

        var query = new GetVehicleMaintenanceOverviewQuery(vehicle.Id);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(vehicle.Id, result.VehicleId);
        Assert.Equal(2, result.TotalSchedules);
        Assert.Equal(1, result.ActiveSchedules);
        Assert.Equal(1, result.DueSoonSchedules);
        Assert.Equal(800m, result.TotalMaintenanceCostLast12Months);
        Assert.Equal(2, result.TotalServicesPerformed);
    }
}
