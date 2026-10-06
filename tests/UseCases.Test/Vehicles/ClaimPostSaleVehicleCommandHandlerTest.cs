using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.ClaimPostSaleVehicle;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.ValueObjects;

namespace UseCases.Test.Vehicles;

public class ClaimPostSaleVehicleCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Claims_Vehicle_Into_Buyer_Personal_Workspace()
    {
        var dealershipTenantId = Guid.NewGuid();
        var dealershipUserId = Guid.NewGuid();

        var buyerTenantId = Guid.NewGuid();
        var buyerUserId = Guid.NewGuid();

        var originalVehicle = VehicleBuilder.Build(
            tenantId: dealershipTenantId,
            userId: dealershipUserId,
            name: "Golf GTI 2024",
            initialOdometer: 12_000
        );

        var sale = VehicleSaleBuilder.Build(
            tenantId: dealershipTenantId,
            userId: dealershipUserId,
            vehicleId: originalVehicle.Id,
            customerName: "Matheus Cruz",
            customerEmail: "matheus@dotcruz.com",
            soldOdometer: 12_000
        );

        var tokenString = "valid.claim.token";
        var portalToken = new PostSalePortalToken(originalVehicle.Id, sale.Id, dealershipTenantId, DateTimeOffset.UtcNow.AddDays(15));

        var tokenService = new CheckinTokenServiceBuilder()
            .SetupValidPostSaleToken(tokenString, portalToken)
            .Build();

        var (_, saleWrite) = new VehicleSaleRepositoryBuilder()
            .SetupGetSaleToUpdateUnscoped(sale)
            .Build();

        Vehicle? claimedVehicleSaved = null;
        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdateUnscoped(originalVehicle)
            .SetupAdd(v => claimedVehicleSaved = v)
            .Build();

        var schedule = MaintenanceScheduleBuilder.Build(
            tenantId: dealershipTenantId,
            vehicleId: originalVehicle.Id,
            description: "Troca de Óleo dos 20.000km"
        );

        var schedulesCopied = new List<MaintenanceSchedule>();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByVehicleIdToUpdateUnscoped(new List<MaintenanceSchedule> { schedule })
            .SetupAdd(s => schedulesCopied.Add(s))
            .Build();

        var tenantProvider = new TenantProviderBuilder()
            .AsUser(buyerTenantId, buyerUserId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new ClaimPostSaleVehicleCommandHandler(
            tokenService,
            saleWrite,
            vehicleWrite,
            scheduleWrite,
            tenantProvider,
            unitOfWork
        );

        var command = new ClaimPostSaleVehicleCommand(tokenString);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal("Golf GTI 2024", result.VehicleName);
        Assert.NotNull(claimedVehicleSaved);
        Assert.Equal(buyerTenantId, claimedVehicleSaved.TenantId);
        Assert.Equal(buyerUserId, claimedVehicleSaved.UserId);
        Assert.Equal(12_000, claimedVehicleSaved.CurrentOdometer);

        // Dealership sale marked as claimed
        Assert.Equal(buyerUserId, sale.ClaimedByUserId);
        Assert.NotNull(sale.ClaimedAt);

        // Schedule was copied to buyer workspace
        Assert.Single(schedulesCopied);
        Assert.Equal(buyerTenantId, schedulesCopied[0].TenantId);
        Assert.Equal(buyerUserId, schedulesCopied[0].UserId);
        Assert.Equal(claimedVehicleSaved.Id, schedulesCopied[0].VehicleId);
    }

    [Fact]
    public async Task Throws_ErrorOnValidationException_When_Vehicle_Already_Claimed()
    {
        var dealershipTenantId = Guid.NewGuid();
        var buyerTenantId = Guid.NewGuid();
        var buyerUserId = Guid.NewGuid();

        var sale = VehicleSaleBuilder.Build(tenantId: dealershipTenantId);
        sale.MarkAsClaimed(Guid.NewGuid(), DateTimeOffset.UtcNow);

        var tokenString = "already.claimed.token";
        var portalToken = new PostSalePortalToken(sale.VehicleId, sale.Id, dealershipTenantId, DateTimeOffset.UtcNow.AddDays(15));

        var tokenService = new CheckinTokenServiceBuilder()
            .SetupValidPostSaleToken(tokenString, portalToken)
            .Build();

        var (_, saleWrite) = new VehicleSaleRepositoryBuilder()
            .SetupGetSaleToUpdateUnscoped(sale)
            .Build();

        var (_, vehicleWrite) = new VehicleRepositoryBuilder().Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();

        var tenantProvider = new TenantProviderBuilder()
            .AsUser(buyerTenantId, buyerUserId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new ClaimPostSaleVehicleCommandHandler(
            tokenService,
            saleWrite,
            vehicleWrite,
            scheduleWrite,
            tenantProvider,
            unitOfWork
        );

        var command = new ClaimPostSaleVehicleCommand(tokenString);

        await Assert.ThrowsAsync<ErrorOnValidationException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));
    }
}
