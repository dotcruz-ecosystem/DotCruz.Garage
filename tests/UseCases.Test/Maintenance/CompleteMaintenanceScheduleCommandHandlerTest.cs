using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.CompleteMaintenanceSchedule;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;

namespace UseCases.Test.Maintenance;

public class CompleteMaintenanceScheduleCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Completes_Schedule_And_Creates_Maintenance_Log()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: workspaceId, userId: userId, initialOdometer: 50_000);
        var schedule = MaintenanceScheduleBuilder.Build(
            workspaceId: workspaceId,
            userId: userId,
            vehicleId: vehicle.Id,
            currentOdometer: 50_000,
            intervalOdometer: 10_000,
            lastPerformedOdometer: 50_000);

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdate(vehicle)
            .Build();

        var scheduleRepo = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByIdToUpdate(schedule)
            .SetupGetByVehicleIdToUpdate(new[] { schedule });
        var (_, scheduleWrite) = scheduleRepo.Build();

        var logRepo = new MaintenanceLogRepositoryBuilder();
        var (_, logWrite) = logRepo.Build();

        var workspaceProvider = new WorkspaceProviderBuilder().AsUser(workspaceId, userId).Build();

        var handler = new CompleteMaintenanceScheduleCommandHandler(
            vehicleWrite,
            scheduleWrite,
            logWrite,
            workspaceProvider,
            UnitOfWorkBuilder.Build());

        var reqDto = new CompleteMaintenanceScheduleRequestDto(
            PerformedAt: DateTimeOffset.UtcNow,
            OdometerAtService: 60_000,
            Cost: 450.00m,
            ReceiptUrl: "https://storage.dotcruz.com/receipts/os-123.pdf",
            Notes: "Óleo sintético 5W30 trocado");

        var command = new CompleteMaintenanceScheduleCommand(vehicle.Id, schedule.Id, reqDto);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(vehicle.Id, result.VehicleId);
        Assert.Equal(schedule.Id, result.MaintenanceScheduleId);
        Assert.Equal(450.00m, result.Cost);
        Assert.Equal(60_000, result.OdometerAtService);
        Assert.Equal(60_000, schedule.LastPerformedOdometer);
        Assert.Equal(70_000, schedule.NextDueOdometer);
        Assert.Equal(60_000, vehicle.CurrentOdometer);
    }

    [Fact]
    public async Task Throws_NotFoundException_When_Vehicle_Not_Found()
    {
        var (_, vehicleWrite) = new VehicleRepositoryBuilder().Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();
        var (_, logWrite) = new MaintenanceLogRepositoryBuilder().Build();
        var workspaceProvider = new WorkspaceProviderBuilder().Build();

        var handler = new CompleteMaintenanceScheduleCommandHandler(
            vehicleWrite,
            scheduleWrite,
            logWrite,
            workspaceProvider,
            UnitOfWorkBuilder.Build());

        var reqDto = new CompleteMaintenanceScheduleRequestDto(DateTimeOffset.UtcNow, 60_000, 100m);
        var command = new CompleteMaintenanceScheduleCommand(Guid.NewGuid(), Guid.NewGuid(), reqDto);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Throws_ErrorOnValidationException_When_Odometer_Is_Lower_Than_Vehicle_Current()
    {
        var workspaceId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: workspaceId, initialOdometer: 50_000);
        var schedule = MaintenanceScheduleBuilder.Build(workspaceId: workspaceId, vehicleId: vehicle.Id, currentOdometer: 50_000);

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdate(vehicle)
            .Build();

        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByIdToUpdate(schedule)
            .Build();

        var (_, logWrite) = new MaintenanceLogRepositoryBuilder().Build();
        var workspaceProvider = new WorkspaceProviderBuilder().Build();

        var handler = new CompleteMaintenanceScheduleCommandHandler(
            vehicleWrite,
            scheduleWrite,
            logWrite,
            workspaceProvider,
            UnitOfWorkBuilder.Build());

        var reqDto = new CompleteMaintenanceScheduleRequestDto(DateTimeOffset.UtcNow, 45_000, 100m); // Lower than 50,000
        var command = new CompleteMaintenanceScheduleCommand(vehicle.Id, schedule.Id, reqDto);

        var ex = await Assert.ThrowsAsync<ErrorOnValidationException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));

        Assert.Contains(ex.GetErrorsMessages(), m => m == ResourceMessagesException.ODOMETER_VALUE_INVALID);
    }
}
