using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.DeleteMaintenanceSchedule;

namespace UseCases.Test.Maintenance;

public class DeleteMaintenanceScheduleCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Soft_Deletes_Schedule()
    {
        var vehicleId = Guid.NewGuid();
        var schedule = MaintenanceScheduleBuilder.Build(vehicleId: vehicleId);

        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByIdToUpdate(schedule)
            .Build();

        var handler = new DeleteMaintenanceScheduleCommandHandler(
            scheduleWrite,
            UnitOfWorkBuilder.Build());

        var command = new DeleteMaintenanceScheduleCommand(vehicleId, schedule.Id);
        await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotNull(schedule.DeletedAt);
    }
}
