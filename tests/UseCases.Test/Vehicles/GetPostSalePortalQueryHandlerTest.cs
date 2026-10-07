using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetPostSalePortal;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.ValueObjects;

namespace UseCases.Test.Vehicles;

public class GetPostSalePortalQueryHandlerTest
{
    [Fact]
    public async Task Successfully_Returns_Portal_Details_For_Valid_Token()
    {
        var workspaceId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: workspaceId, name: "Civic Touring", initialOdometer: 55_000);
        var sale = VehicleSaleBuilder.Build(
            workspaceId: workspaceId,
            vehicleId: vehicle.Id,
            customerName: "Lucas Mendes",
            soldOdometer: 50_000,
            warrantyKm: 5000,
            warrantyExpiresAt: DateTimeOffset.UtcNow.AddMonths(2)
        );

        var tokenString = "valid.postsale.portal.token";
        var portalToken = new PostSalePortalToken(vehicle.Id, sale.Id, workspaceId, DateTimeOffset.UtcNow.AddDays(30));

        var tokenService = new CheckinTokenServiceBuilder()
            .SetupValidPostSaleToken(tokenString, portalToken)
            .Build();

        var (saleRead, _) = new VehicleSaleRepositoryBuilder()
            .SetupGetSaleUnscoped(sale)
            .Build();

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithHistoriesToUpdateUnscoped(vehicle)
            .Build();

        var schedule = MaintenanceScheduleBuilder.Build(workspaceId: workspaceId, vehicleId: vehicle.Id, description: "Revisão Geral");
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByVehicleIdToUpdateUnscoped(new List<MaintenanceSchedule> { schedule })
            .Build();

        var (logRead, _) = new MaintenanceLogRepositoryBuilder()
            .SetupGetByVehicleIdUnscoped(new List<MaintenanceLog>())
            .Build();

        var handler = new GetPostSalePortalQueryHandler(
            tokenService,
            saleRead,
            vehicleWrite,
            scheduleWrite,
            logRead
        );

        var query = new GetPostSalePortalQuery(tokenString);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(vehicle.Id, result.VehicleId);
        Assert.Equal("Civic Touring", result.VehicleName);
        Assert.Equal("Lucas Mendes", result.CustomerName);
        Assert.Equal(55_000, result.CurrentOdometer);
        Assert.True(result.IsWarrantyActive);
        Assert.Single(result.UpcomingMaintenance);
        Assert.Equal("Revisão Geral", result.UpcomingMaintenance.First().Description);
    }

    [Fact]
    public async Task Throws_ErrorOnValidationException_When_Token_Is_Invalid()
    {
        var tokenString = "invalid.token";
        var tokenService = new CheckinTokenServiceBuilder()
            .SetupInvalidPostSaleToken(tokenString)
            .Build();

        var (saleRead, _) = new VehicleSaleRepositoryBuilder().Build();
        var (_, vehicleWrite) = new VehicleRepositoryBuilder().Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();
        var (logRead, _) = new MaintenanceLogRepositoryBuilder().Build();

        var handler = new GetPostSalePortalQueryHandler(
            tokenService,
            saleRead,
            vehicleWrite,
            scheduleWrite,
            logRead
        );

        var query = new GetPostSalePortalQuery(tokenString);

        await Assert.ThrowsAsync<ErrorOnValidationException>(() =>
            handler.Handle(query, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Throws_ErrorOnValidationException_When_Token_Is_Expired()
    {
        var tokenString = "expired.token";
        var portalToken = new PostSalePortalToken(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow.AddMinutes(-5));

        var tokenService = new CheckinTokenServiceBuilder()
            .SetupValidPostSaleToken(tokenString, portalToken)
            .Build();

        var (saleRead, _) = new VehicleSaleRepositoryBuilder().Build();
        var (_, vehicleWrite) = new VehicleRepositoryBuilder().Build();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();
        var (logRead, _) = new MaintenanceLogRepositoryBuilder().Build();

        var handler = new GetPostSalePortalQueryHandler(
            tokenService,
            saleRead,
            vehicleWrite,
            scheduleWrite,
            logRead
        );

        var query = new GetPostSalePortalQuery(tokenString);

        await Assert.ThrowsAsync<ErrorOnValidationException>(() =>
            handler.Handle(query, TestContext.Current.CancellationToken));
    }
}
