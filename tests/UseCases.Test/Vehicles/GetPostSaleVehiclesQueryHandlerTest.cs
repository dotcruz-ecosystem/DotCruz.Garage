using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetPostSaleVehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;

namespace UseCases.Test.Vehicles;

public class GetPostSaleVehiclesQueryHandlerTest
{
    [Fact]
    public async Task Successfully_Returns_Active_Sales_With_Vehicle_Details()
    {
        var tenantId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(tenantId: tenantId, initialOdometer: 60_000);
        var sale = VehicleSaleBuilder.Build(tenantId: tenantId, vehicleId: vehicle.Id, customerName: "Fernanda Costa", soldOdometer: 58_000);

        var (saleRead, _) = new VehicleSaleRepositoryBuilder()
            .SetupGetActiveSalesWithVehicle(new List<VehicleSale> { sale })
            .Build();

        var handler = new GetPostSaleVehiclesQueryHandler(saleRead);

        var result = await handler.Handle(new GetPostSaleVehiclesQuery(), TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Single(result);
        var first = result.First();
        Assert.Equal("Fernanda Costa", first.CustomerName);
        Assert.Equal(58_000, first.SoldOdometer);
        Assert.True(first.IsActive);
    }
}
