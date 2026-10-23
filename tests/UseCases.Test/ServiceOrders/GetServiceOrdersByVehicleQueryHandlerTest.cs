using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.GetServiceOrdersByVehicle;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.ServiceOrders;

public class GetServiceOrdersByVehicleQueryHandlerTest
{
    [Fact]
    public async Task Returns_Paginated_ServiceOrders()
    {
        var vehicle = VehicleBuilder.Build();
        var so1 = ServiceOrderBuilder.Build(vehicleId: vehicle.Id);
        var so2 = ServiceOrderBuilder.Build(vehicleId: vehicle.Id);

        var (vehicleRead, _) = new VehicleRepositoryBuilder()
            .SetupGetById(vehicle)
            .Build();

        var (soRead, _) = new ServiceOrderRepositoryBuilder()
            .SetupGetByVehicleIdPaginated(new[] { so1, so2 }, 2)
            .Build();

        var handler = new GetServiceOrdersByVehicleQueryHandler(vehicleRead, soRead);
        var query = new GetServiceOrdersByVehicleQuery(vehicle.Id, 1, 10);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task Throws_NotFound_When_Vehicle_Does_Not_Exist()
    {
        var (vehicleRead, _) = new VehicleRepositoryBuilder()
            .SetupGetById(null)
            .Build();

        var (soRead, _) = new ServiceOrderRepositoryBuilder().Build();

        var handler = new GetServiceOrdersByVehicleQueryHandler(vehicleRead, soRead);
        var query = new GetServiceOrdersByVehicleQuery(Guid.NewGuid(), 1, 10);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(query, TestContext.Current.CancellationToken));
    }
}
