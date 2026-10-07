using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Reports.Queries.GetFleetTcoSummary;
using DotCruz.Garage.Domain.Enums;

namespace UseCases.Test.Reports;

public class GetFleetTcoSummaryQueryHandlerTest
{
    [Fact]
    public async Task Successfully_Calculates_Fleet_Tco_And_Ranking()
    {
        var workspaceId = Guid.NewGuid();

        // Vehicle 1: Initial 50.000, Current 60.000 => 10.000 KM traveled
        var vehicle1 = VehicleBuilder.Build(workspaceId: workspaceId, initialOdometer: 50_000, plate: "ABC-1234");
        typeof(DotCruz.Garage.Domain.Entities.Vehicles.Vehicle)
            .GetProperty(nameof(vehicle1.CurrentOdometer))!
            .SetValue(vehicle1, 60_000);

        // Vehicle 2: Initial 10.000, Current 15.000 => 5.000 KM traveled
        var vehicle2 = VehicleBuilder.Build(workspaceId: workspaceId, initialOdometer: 10_000, plate: "XYZ-9876");
        typeof(DotCruz.Garage.Domain.Entities.Vehicles.Vehicle)
            .GetProperty(nameof(vehicle2.CurrentOdometer))!
            .SetValue(vehicle2, 15_000);

        // Vehicle 1 costs: Maintenance 1000, Fine 200, Doc 1500, Accessory 500 => Total 3200
        var log1 = MaintenanceLogBuilder.Build(workspaceId: workspaceId, vehicleId: vehicle1.Id, cost: 1000m, performedAt: DateTimeOffset.UtcNow);
        var fine1 = TrafficFineBuilder.Build(workspaceId: workspaceId, vehicleId: vehicle1.Id, amount: 200m, infractionDate: DateTimeOffset.UtcNow);
        var doc1 = VehicleDocumentBuilder.Build(workspaceId: workspaceId, vehicleId: vehicle1.Id, cost: 1500m, dueDate: DateTimeOffset.UtcNow);
        var acc1 = VehicleAccessoryBuilder.Build(workspaceId: workspaceId, vehicleId: vehicle1.Id, cost: 500m, purchaseDate: DateTimeOffset.UtcNow);

        // Vehicle 2 costs: Maintenance 400, Fine 0, Doc 800, Accessory 0 => Total 1200
        var log2 = MaintenanceLogBuilder.Build(workspaceId: workspaceId, vehicleId: vehicle2.Id, cost: 400m, performedAt: DateTimeOffset.UtcNow);
        var doc2 = VehicleDocumentBuilder.Build(workspaceId: workspaceId, vehicleId: vehicle2.Id, cost: 800m, dueDate: DateTimeOffset.UtcNow);

        var (vRead, _) = new VehicleRepositoryBuilder().SetupGetAll(new[] { vehicle1, vehicle2 }).Build();
        var (mRead, _) = new MaintenanceLogRepositoryBuilder().SetupGetAll(new[] { log1, log2 }).Build();
        var (fRead, _) = new TrafficFineRepositoryBuilder().SetupGetAll(new[] { fine1 }).Build();
        var (dRead, _) = new VehicleDocumentRepositoryBuilder().SetupGetAll(new[] { doc1, doc2 }).Build();
        var (aRead, _) = new VehicleAccessoryRepositoryBuilder().SetupGetAll(new[] { acc1 }).Build();

        var handler = new GetFleetTcoSummaryQueryHandler(vRead, mRead, fRead, dRead, aRead);

        var query = new GetFleetTcoSummaryQuery();
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalVehicles);
        Assert.Equal(15_000, result.TotalFleetDistanceKm);
        Assert.Equal(4400m, result.TotalFleetCost);
        Assert.Equal(1400m, result.TotalMaintenanceCost);
        Assert.Equal(200m, result.TotalFinesCost);
        Assert.Equal(2300m, result.TotalDocumentsCost);
        Assert.Equal(500m, result.TotalAccessoriesCost);
        Assert.Equal(Math.Round(4400m / 15000m, 4), result.AverageCostPerKm);

        Assert.Equal(2, result.VehicleRanking.Count);
        Assert.Equal(vehicle1.Id, result.VehicleRanking.First().VehicleId); // 3200 > 1200
        Assert.Equal(3200m, result.VehicleRanking.First().TotalCost);
        Assert.Equal(1200m, result.VehicleRanking.Last().TotalCost);
    }
}
