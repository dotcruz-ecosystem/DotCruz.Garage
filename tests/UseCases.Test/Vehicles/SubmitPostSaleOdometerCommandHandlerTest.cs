using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.SubmitPostSaleOdometer;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.ValueObjects;

namespace UseCases.Test.Vehicles;

public class SubmitPostSaleOdometerCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Submits_Odometer_And_Recalculates_Schedules()
    {
        var tenantId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(tenantId: tenantId, initialOdometer: 50_000);
        var saleId = Guid.NewGuid();

        var tokenString = "valid.postsale.portal.token";
        var portalToken = new PostSalePortalToken(vehicle.Id, saleId, tenantId, DateTimeOffset.UtcNow.AddDays(10));

        var tokenService = new CheckinTokenServiceBuilder()
            .SetupValidPostSaleToken(tokenString, portalToken)
            .Build();

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdateUnscoped(vehicle)
            .Build();

        var schedule = MaintenanceScheduleBuilder.Build(
            tenantId: tenantId,
            vehicleId: vehicle.Id,
            intervalOdometer: 10_000,
            lastPerformedOdometer: 50_000
        );

        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByVehicleIdToUpdateUnscoped(new List<MaintenanceSchedule> { schedule })
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new SubmitPostSaleOdometerCommandHandler(
            tokenService,
            vehicleWrite,
            scheduleWrite,
            unitOfWork
        );

        var command = new SubmitPostSaleOdometerCommand(tokenString, 53_000);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(50_000, result.OldOdometer);
        Assert.Equal(53_000, result.NewOdometer);
        Assert.Equal(53_000, vehicle.CurrentOdometer);
        Assert.True(result.SchedulesRecalculated);
    }

    [Fact]
    public async Task Throws_ErrorOnValidationException_When_Odometer_Is_Lower_Than_Current()
    {
        var tenantId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(tenantId: tenantId, initialOdometer: 50_000);
        var saleId = Guid.NewGuid();

        var tokenString = "valid.postsale.portal.token";
        var portalToken = new PostSalePortalToken(vehicle.Id, saleId, tenantId, DateTimeOffset.UtcNow.AddDays(10));

        var tokenService = new CheckinTokenServiceBuilder()
            .SetupValidPostSaleToken(tokenString, portalToken)
            .Build();

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdateUnscoped(vehicle)
            .Build();

        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new SubmitPostSaleOdometerCommandHandler(
            tokenService,
            vehicleWrite,
            scheduleWrite,
            unitOfWork
        );

        var command = new SubmitPostSaleOdometerCommand(tokenString, 49_000);

        await Assert.ThrowsAsync<ErrorOnValidationException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Throws_ErrorOnValidationException_When_Odometer_Jump_Is_Unrealistic()
    {
        var tenantId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(tenantId: tenantId, initialOdometer: 50_000);
        var saleId = Guid.NewGuid();

        var tokenString = "valid.postsale.portal.token";
        var portalToken = new PostSalePortalToken(vehicle.Id, saleId, tenantId, DateTimeOffset.UtcNow.AddDays(10));

        var tokenService = new CheckinTokenServiceBuilder()
            .SetupValidPostSaleToken(tokenString, portalToken)
            .Build();

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdateUnscoped(vehicle)
            .Build();

        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new SubmitPostSaleOdometerCommandHandler(
            tokenService,
            vehicleWrite,
            scheduleWrite,
            unitOfWork
        );

        // Jump of 30,000 km exceeds max realistic threshold (20,000 km)
        var command = new SubmitPostSaleOdometerCommand(tokenString, 80_001);

        await Assert.ThrowsAsync<ErrorOnValidationException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));
    }
}
