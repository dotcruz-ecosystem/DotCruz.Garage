using CommonTestUtilities.Entities;
using CommonTestUtilities.Services;
using DotCruz.Garage.Infrastructure.Data;
using DotCruz.Garage.Infrastructure.Data.Repositories.Maintenance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Test.Data;

public class JobQueryScopeTest
{
    [Fact]
    public async Task Job_Reaches_Schedules_From_Every_Workspace()
    {
        var workspaceA = Guid.NewGuid();
        var workspaceB = Guid.NewGuid();

        await using var context = BuildContextWithoutWorkspace();
        SeedDueSchedule(context, workspaceA);
        SeedDueSchedule(context, workspaceB);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new MaintenanceScheduleWriteRepository(context);

        var due = await repository.GetSchedulesWithVehicleDetailsToUpdateAsync(TestContext.Current.CancellationToken);

        Assert.Equal(2, due.Count);
        Assert.Contains(due, s => s.OriginWorkspaceId == workspaceA);
        Assert.Contains(due, s => s.OriginWorkspaceId == workspaceB);
    }

    [Fact]
    public async Task Job_Skips_Soft_Deleted_Schedules()
    {
        await using var context = BuildContextWithoutWorkspace();

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
    public async Task Workspace_Scope_Still_Applies_To_Regular_Vehicle_Queries()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var databaseName = Guid.NewGuid().ToString();

        await using (var seedContext = BuildContext(databaseName, new WorkspaceProviderBuilder().AsService().Build()))
        {
            SeedDueSchedule(seedContext, workspaceId, userId);
            SeedDueSchedule(seedContext, Guid.NewGuid());
            await seedContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using var scopedContext = BuildContext(
            databaseName,
            new WorkspaceProviderBuilder().AsUser(workspaceId, userId).Build());

        var repository = new DotCruz.Garage.Infrastructure.Data.Repositories.Vehicles.VehicleReadRepository(scopedContext);
        var (items, totalCount) = await repository.SearchAsync(1, 10, null, TestContext.Current.CancellationToken);

        Assert.Single(items);
        Assert.Equal(1, totalCount);
    }

    private static GarageDbContext BuildContextWithoutWorkspace() =>
        BuildContext(Guid.NewGuid().ToString(), new WorkspaceProviderBuilder().AsService().Build());

    private static GarageDbContext BuildContext(string databaseName, DotCruz.Shared.Security.Context.IWorkspaceProvider workspaceProvider)
    {
        var options = new DbContextOptionsBuilder<GarageDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new GarageDbContext(options, workspaceProvider);
    }

    private static DotCruz.Garage.Domain.Entities.Maintenance.MaintenanceSchedule SeedDueSchedule(
        GarageDbContext context,
        Guid workspaceId,
        Guid? userId = null)
    {
        var owner = userId ?? Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: workspaceId, userId: owner, initialOdometer: 50_000);
        var schedule = MaintenanceScheduleBuilder.Build(
            workspaceId: workspaceId,
            userId: owner,
            vehicleId: vehicle.Id,
            currentOdometer: 50_000);

        context.Vehicles.Add(vehicle);
        context.MaintenanceSchedules.Add(schedule);

        return schedule;
    }
}
