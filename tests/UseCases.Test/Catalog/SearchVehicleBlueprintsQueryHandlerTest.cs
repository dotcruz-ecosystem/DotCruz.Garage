using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Catalog.Queries.SearchVehicleBlueprints;

namespace UseCases.Test.Catalog;

public class SearchVehicleBlueprintsQueryHandlerTest
{
    [Fact]
    public async Task Returns_Paged_List_Of_Blueprints()
    {
        var blueprints = new[]
        {
            VehicleBlueprintBuilder.Build(brand: "Fiat", model: "Strada"),
            VehicleBlueprintBuilder.Build(brand: "Fiat", model: "Toro")
        };

        var (blueprintRead, _) = new VehicleBlueprintRepositoryBuilder()
            .SetupSearch(blueprints, 2)
            .Build();

        var handler = new SearchVehicleBlueprintsQueryHandler(blueprintRead);
        var result = await handler.Handle(
            new SearchVehicleBlueprintsQuery(PageNumber: 1, PageSize: 10, SearchTerm: "Fiat"),
            TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }
}
