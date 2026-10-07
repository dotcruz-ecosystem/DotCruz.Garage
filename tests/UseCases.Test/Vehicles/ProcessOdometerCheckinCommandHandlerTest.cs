using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.ProcessOdometerCheckin;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Garage.Domain.ValueObjects;
using Moq;

namespace UseCases.Test.Vehicles;

public class ProcessOdometerCheckinCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Updates_Odometer_And_Recalculates_Schedules()
    {
        var workspaceId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: workspaceId, initialOdometer: 50_000);
        var schedule = MaintenanceScheduleBuilder.Build(
            workspaceId: workspaceId,
            vehicleId: vehicle.Id,
            currentOdometer: 50_000,
            intervalOdometer: 10_000);

        var tokenVo = new OdometerCheckinToken(vehicle.Id, workspaceId, DateTimeOffset.UtcNow.AddHours(24));
        var tokenService = new CheckinTokenServiceBuilder()
            .SetupValidToken("valid.token", tokenVo);

        var vehicleRepo = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdateUnscoped(vehicle);
        var (_, vehicleWrite) = vehicleRepo.Build();

        var scheduleRepo = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByVehicleIdToUpdateUnscoped(new[] { schedule });
        var (_, scheduleWrite) = scheduleRepo.Build();

        var handler = new ProcessOdometerCheckinCommandHandler(
            tokenService.Build(),
            vehicleWrite,
            scheduleWrite,
            UnitOfWorkBuilder.Build());

        var command = new ProcessOdometerCheckinCommand("valid.token", 55_000, DateTimeOffset.UtcNow);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(vehicle.Id, result.VehicleId);
        Assert.Equal(50_000, result.OldOdometer);
        Assert.Equal(55_000, result.NewOdometer);
        Assert.Equal(55_000, vehicle.CurrentOdometer);
        Assert.True(result.SchedulesRecalculated);
        Assert.Equal(2, vehicle.OdometerHistories.Count);
    }

    [Fact]
    public async Task Throws_ErrorOnValidationException_When_Token_Is_Invalid()
    {
        var tokenService = new CheckinTokenServiceBuilder()
            .SetupInvalidToken("tampered.token");

        var (_, vehicleWrite) = new VehicleRepositoryBuilder().Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();

        var handler = new ProcessOdometerCheckinCommandHandler(
            tokenService.Build(),
            vehicleWrite,
            scheduleWrite,
            UnitOfWorkBuilder.Build());

        var command = new ProcessOdometerCheckinCommand("tampered.token", 55_000);

        var ex = await Assert.ThrowsAsync<ErrorOnValidationException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));

        Assert.Contains(ex.GetErrorsMessages(), m => m == ResourceMessagesException.TOKEN_INVALID);
    }

    [Fact]
    public async Task Throws_ErrorOnValidationException_When_Token_Is_Expired()
    {
        var vehicleId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var expiredTokenVo = new OdometerCheckinToken(vehicleId, workspaceId, DateTimeOffset.UtcNow.AddHours(-1));

        var tokenService = new CheckinTokenServiceBuilder()
            .SetupValidToken("expired.token", expiredTokenVo);

        var (_, vehicleWrite) = new VehicleRepositoryBuilder().Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();

        var handler = new ProcessOdometerCheckinCommandHandler(
            tokenService.Build(),
            vehicleWrite,
            scheduleWrite,
            UnitOfWorkBuilder.Build());

        var command = new ProcessOdometerCheckinCommand("expired.token", 55_000);

        var ex = await Assert.ThrowsAsync<ErrorOnValidationException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));

        Assert.Contains(ex.GetErrorsMessages(), m => m == ResourceMessagesException.TOKEN_EXPIRED);
    }

    [Fact]
    public async Task Throws_ErrorOnValidationException_When_New_Odometer_Is_Less_Than_Current()
    {
        var workspaceId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: workspaceId, initialOdometer: 50_000);
        var tokenVo = new OdometerCheckinToken(vehicle.Id, workspaceId, DateTimeOffset.UtcNow.AddHours(24));

        var tokenService = new CheckinTokenServiceBuilder().SetupValidToken("valid.token", tokenVo);
        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdateUnscoped(vehicle)
            .Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();

        var handler = new ProcessOdometerCheckinCommandHandler(
            tokenService.Build(),
            vehicleWrite,
            scheduleWrite,
            UnitOfWorkBuilder.Build());

        var command = new ProcessOdometerCheckinCommand("valid.token", 49_000);

        var ex = await Assert.ThrowsAsync<ErrorOnValidationException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));

        Assert.Contains(ex.GetErrorsMessages(), m => m == ResourceMessagesException.ODOMETER_VALUE_INVALID);
    }

    [Fact]
    public async Task Throws_ErrorOnValidationException_When_Odometer_Jump_Is_Unrealistic()
    {
        var workspaceId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: workspaceId, initialOdometer: 50_000);
        var tokenVo = new OdometerCheckinToken(vehicle.Id, workspaceId, DateTimeOffset.UtcNow.AddHours(24));

        var tokenService = new CheckinTokenServiceBuilder().SetupValidToken("valid.token", tokenVo);
        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdateUnscoped(vehicle)
            .Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();

        var handler = new ProcessOdometerCheckinCommandHandler(
            tokenService.Build(),
            vehicleWrite,
            scheduleWrite,
            UnitOfWorkBuilder.Build());

        var command = new ProcessOdometerCheckinCommand("valid.token", 75_000); // 25,000 km jump > 20,000 max

        var ex = await Assert.ThrowsAsync<ErrorOnValidationException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));

        Assert.Contains(ex.GetErrorsMessages(), m => m == ResourceMessagesException.ODOMETER_UNREALISTIC_JUMP);
    }
}
