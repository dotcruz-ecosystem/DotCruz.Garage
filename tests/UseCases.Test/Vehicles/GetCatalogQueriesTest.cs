using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogBrands;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogEngines;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogModels;
using DotCruz.Garage.Domain.Enums;

namespace UseCases.Test.Vehicles;

public class GetCatalogQueriesTest
{
    [Fact]
    public async Task GetCatalogBrands_Returns_Brands_From_Repository()
    {
        var expectedBrands = new List<string> { "Fiat", "Honda", "Toyota" };
        var repo = new VehicleCatalogRepositoryBuilder()
            .SetupGetBrands(expectedBrands)
            .Build();

        var handler = new GetCatalogBrandsQueryHandler(repo);
        var result = await handler.Handle(new GetCatalogBrandsQuery(VehicleType.Car), TestContext.Current.CancellationToken);

        Assert.Equal(expectedBrands, result);
    }

    [Fact]
    public async Task GetCatalogModels_Returns_Models_From_Repository()
    {
        var expectedModels = new List<string> { "Corolla", "Hilux", "Yaris" };
        var repo = new VehicleCatalogRepositoryBuilder()
            .SetupGetModels(expectedModels)
            .Build();

        var handler = new GetCatalogModelsQueryHandler(repo);
        var result = await handler.Handle(new GetCatalogModelsQuery("Toyota", VehicleType.Car), TestContext.Current.CancellationToken);

        Assert.Equal(expectedModels, result);
    }

    [Fact]
    public async Task GetCatalogModels_Returns_Empty_When_Brand_Is_Whitespace()
    {
        var repo = new VehicleCatalogRepositoryBuilder().Build();

        var handler = new GetCatalogModelsQueryHandler(repo);
        var result = await handler.Handle(new GetCatalogModelsQuery("   ", VehicleType.Car), TestContext.Current.CancellationToken);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCatalogEngines_Returns_Engines_From_Repository()
    {
        var expectedEngines = new List<string> { "1.0", "1.6", "2.0" };
        var repo = new VehicleCatalogRepositoryBuilder()
            .SetupGetEngines(expectedEngines)
            .Build();

        var handler = new GetCatalogEnginesQueryHandler(repo);
        var result = await handler.Handle(new GetCatalogEnginesQuery(VehicleType.Car), TestContext.Current.CancellationToken);

        Assert.Equal(expectedEngines, result);
    }
}
