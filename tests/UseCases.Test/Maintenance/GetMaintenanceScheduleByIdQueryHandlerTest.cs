using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetMaintenanceScheduleById;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.Maintenance;

public class GetMaintenanceScheduleByIdQueryHandlerTest
{
    [Fact]
    public async Task Successfully_Returns_Schedule_Dto()
    {
        var vehicleId = Guid.NewGuid();
        var schedule = MaintenanceScheduleBuilder.Build(vehicleId: vehicleId);

        var (scheduleRead, _) = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetById(schedule)
            .Build();

        var handler = new GetMaintenanceScheduleByIdQueryHandler(scheduleRead);

        var query = new GetMaintenanceScheduleByIdQuery(vehicleId, schedule.Id);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(schedule.Id, result.Id);
        Assert.Equal(vehicleId, result.VehicleId);
        Assert.Equal(schedule.Description, result.Description);
    }

    [Fact]
    public async Task Throws_NotFoundException_When_Schedule_Not_Found()
    {
        var (scheduleRead, _) = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetById(null)
            .Build();

        var handler = new GetMaintenanceScheduleByIdQueryHandler(scheduleRead);

        var query = new GetMaintenanceScheduleByIdQuery(Guid.NewGuid(), Guid.NewGuid());
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(query, TestContext.Current.CancellationToken));
    }
}
