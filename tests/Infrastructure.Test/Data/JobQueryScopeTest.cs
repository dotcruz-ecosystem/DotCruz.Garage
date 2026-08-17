using CommonTestUtilities.Entities;
using CommonTestUtilities.Services;
using DotCruz.Garage.Infrastructure.Data;
using DotCruz.Garage.Infrastructure.Data.Repositories.Maintenance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Test.Data;

public class JobQueryScopeTest
{
    [Fact]
    public async Task Job_Reaches_Schedules_From_Every_Tenant()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var context = BuildContextWithoutTenant();
        SeedDueSchedule(context, tenantA);
        SeedDueSchedule(context, tenantB);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new MaintenanceScheduleWriteRepository(context);

        var due = await repository.GetSchedulesWithVehicleDetailsToUpdateAsync(TestContext.Current.CancellationToken);

        Assert.Equal(2, due.Count);
        Assert.Contains(due, s => s.TenantId == tenantA);
        Assert.Contains(due, s => s.TenantId == tenantB);
    }

    [Fact]
    public async Task Job_Skips_Soft_Deleted_Schedules()
    {
        await using var context = BuildContextWithoutTenant();

        var kept = SeedDueSchedule(context, Guid.NewGuid());
        var removed = SeedDueSchedule(context, Guid.NewGuid());
        removed.Delete();

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new MaintenanceScheduleWriteRepository(context);

        var due = await repository.GetSchedulesWithVehicleDetailsToUpdateAsync(TestContext.Current.CancellationToken);

        Assert.Single(due);
        Assert.Equal(kept.Id, due.Single().Id);
    }

    [Fact]
    public async Task Tenant_Scope_Still_Applies_To_Regular_Queries()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var databaseName = Guid.NewGuid().ToString();

        await using (var seedContext = BuildContext(databaseName, new TenantProviderBuilder().AsService().Build()))
        {
            SeedDueSchedule(seedContext, tenantId, userId);
            SeedDueSchedule(seedContext, Guid.NewGuid());
            await seedContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using var scopedContext = BuildContext(
            databaseName,
            new TenantProviderBuilder().AsUser(tenantId, userId).Build());

        var visible = await scopedContext.MaintenanceSchedules.ToListAsync(TestContext.Current.CancellationToken);

        Assert.Single(visible);
        Assert.Equal(tenantId, visible.Single().TenantId);
    }

    private static GarageDbContext BuildContextWithoutTenant() =>
        BuildContext(Guid.NewGuid().ToString(), new TenantProviderBuilder().AsService().Build());

    private static GarageDbContext BuildContext(string databaseName, DotCruz.Shared.Security.Context.ITenantProvider tenantProvider)
    {
        var options = new DbContextOptionsBuilder<GarageDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new GarageDbContext(options, tenantProvider);
    }

    private static DotCruz.Garage.Domain.Entities.Maintenance.MaintenanceSchedule SeedDueSchedule(
        GarageDbContext context,
        Guid tenantId,
        Guid? userId = null)
    {
        var owner = userId ?? Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(tenantId: tenantId, userId: owner, initialOdometer: 50_000);
        var schedule = MaintenanceScheduleBuilder.Build(
            tenantId: tenantId,
            userId: owner,
            vehicleId: vehicle.Id,
            currentOdometer: 50_000);

        context.Vehicles.Add(vehicle);
        context.MaintenanceSchedules.Add(schedule);

        return schedule;
    }
}
