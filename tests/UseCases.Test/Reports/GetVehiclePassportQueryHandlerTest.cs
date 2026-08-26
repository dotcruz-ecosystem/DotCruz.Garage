using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Reports.Queries.GetVehiclePassport;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.Reports;

public class GetVehiclePassportQueryHandlerTest
{
    [Fact]
    public async Task Successfully_Generates_Vehicle_Passport_With_All_Details()
    {
        var tenantId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(tenantId: tenantId, initialOdometer: 40_000);
        typeof(DotCruz.Garage.Domain.Entities.Vehicles.Vehicle)
            .GetProperty(nameof(vehicle.CurrentOdometer))!
            .SetValue(vehicle, 55_000);

        var driver = DriverBuilder.Build(tenantId: tenantId, name: "Roberto Firmino");
        var assignment = VehicleDriverAssignmentBuilder.Build(
            tenantId: tenantId,
            vehicleId: vehicle.Id,
            driverId: driver.Id);
        typeof(DotCruz.Garage.Domain.Entities.Drivers.VehicleDriverAssignment)
            .GetProperty(nameof(assignment.Driver))!
            .SetValue(assignment, driver);

        var log = MaintenanceLogBuilder.Build(
            tenantId: tenantId,
            vehicleId: vehicle.Id,
            cost: 850m,
            odometerAtService: 50_000,
            performedAt: DateTimeOffset.UtcNow.AddMonths(-1));

        var schedule = MaintenanceScheduleBuilder.Build(
            tenantId: tenantId,
            vehicleId: vehicle.Id,
            currentOdometer: 55_000,
            intervalOdometer: 10_000,
            lastPerformedOdometer: 50_000);

        var doc = VehicleDocumentBuilder.Build(
            tenantId: tenantId,
            vehicleId: vehicle.Id,
            cost: 1400m,
            type: DocumentType.IPVA);

        var fine = TrafficFineBuilder.Build(
            tenantId: tenantId,
            vehicleId: vehicle.Id,
            amount: 195m);

        var acc = VehicleAccessoryBuilder.Build(
            tenantId: tenantId,
            vehicleId: vehicle.Id,
            cost: 300m);

        var (vRead, _) = new VehicleRepositoryBuilder().SetupGetById(vehicle).Build();
        var (assignRead, _) = new VehicleDriverAssignmentRepositoryBuilder().SetupGetCurrentByVehicleId(assignment).Build();
        var (mRead, _) = new MaintenanceLogRepositoryBuilder().SetupGetByVehicleIdPaginated(new[] { log }, 1).Build();
        var (sRead, _) = new MaintenanceScheduleRepositoryBuilder().SetupGetByVehicleId(new[] { schedule }).Build();
        var (dRead, _) = new VehicleDocumentRepositoryBuilder().SetupGetByVehicleId(new[] { doc }).Build();
        var (fRead, _) = new TrafficFineRepositoryBuilder().SetupGetByVehicleId(new[] { fine }).Build();
        var (aRead, _) = new VehicleAccessoryRepositoryBuilder().SetupGetByVehicleId(new[] { acc }).Build();

        var handler = new GetVehiclePassportQueryHandler(
            vRead,
            assignRead,
            mRead,
            sRead,
            dRead,
            fRead,
            aRead);

        var query = new GetVehiclePassportQuery(vehicle.Id);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(vehicle.Id, result.VehicleId);
        Assert.Equal("Roberto Firmino", result.CurrentDriverName);
        Assert.Equal(15_000, result.TotalDistanceTraveledKm);
        Assert.Equal(850m, result.FinancialSummary.TotalMaintenanceCost);
        Assert.Equal(195m, result.FinancialSummary.TotalFinesCost);
        Assert.Equal(1400m, result.FinancialSummary.TotalDocumentsCost);
        Assert.Equal(300m, result.FinancialSummary.TotalAccessoriesCost);
        Assert.Equal(2745m, result.FinancialSummary.TotalTco);
        Assert.StartsWith("DC-PASSPORT-", result.VerificationCode);
        Assert.Single(result.MaintenanceHistory);
        Assert.Single(result.UpcomingMaintenances);
        Assert.Single(result.Documents);
    }

    [Fact]
    public async Task Throws_NotFoundException_When_Vehicle_Not_Found()
    {
        var (vRead, _) = new VehicleRepositoryBuilder().SetupGetById(null).Build();
        var (assignRead, _) = new VehicleDriverAssignmentRepositoryBuilder().Build();
        var (mRead, _) = new MaintenanceLogRepositoryBuilder().Build();
        var (sRead, _) = new MaintenanceScheduleRepositoryBuilder().Build();
        var (dRead, _) = new VehicleDocumentRepositoryBuilder().Build();
        var (fRead, _) = new TrafficFineRepositoryBuilder().Build();
        var (aRead, _) = new VehicleAccessoryRepositoryBuilder().Build();

        var handler = new GetVehiclePassportQueryHandler(
            vRead,
            assignRead,
            mRead,
            sRead,
            dRead,
            fRead,
            aRead);

        var query = new GetVehiclePassportQuery(Guid.NewGuid());
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(query, TestContext.Current.CancellationToken));
    }
}
