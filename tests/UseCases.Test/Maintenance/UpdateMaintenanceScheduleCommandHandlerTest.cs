using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.UpdateMaintenanceSchedule;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.Maintenance;

public class UpdateMaintenanceScheduleCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Updates_Schedule()
    {
        var workspaceId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: workspaceId, initialOdometer: 50_000);
        var schedule = MaintenanceScheduleBuilder.Build(workspaceId: workspaceId, vehicleId: vehicle.Id, currentOdometer: 50_000);

        var (vehicleRead, _) = new VehicleRepositoryBuilder().SetupGetById(vehicle).Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().SetupGetByIdToUpdate(schedule).Build();

        var handler = new UpdateMaintenanceScheduleCommandHandler(
            vehicleRead,
            scheduleWrite,
            UnitOfWorkBuilder.Build());

        var reqDto = new UpdateMaintenanceScheduleRequestDto(
            Description: "Nova Descrição Atualizada",
            Category: MaintenanceCategory.Brakes,
            IntervalOdometer: 15_000);

        var command = new UpdateMaintenanceScheduleCommand(vehicle.Id, schedule.Id, reqDto);
        await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.Equal("Nova Descrição Atualizada", schedule.Description);
        Assert.Equal(MaintenanceCategory.Brakes, schedule.Category);
        Assert.Equal(15_000, schedule.IntervalOdometer);
    }
}
