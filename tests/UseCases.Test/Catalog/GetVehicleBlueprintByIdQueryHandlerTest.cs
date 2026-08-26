using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Catalog.Queries.GetVehicleBlueprintById;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.Catalog;

public class GetVehicleBlueprintByIdQueryHandlerTest
{
    [Fact]
    public async Task Returns_Blueprint_Dto_When_Found()
    {
        var blueprint = VehicleBlueprintBuilder.Build(brand: "Honda", model: "Civic", year: 2022, templatesCount: 2);
        var (blueprintRead, _) = new VehicleBlueprintRepositoryBuilder().SetupGetById(blueprint).Build();

        var handler = new GetVehicleBlueprintByIdQueryHandler(blueprintRead);
        var result = await handler.Handle(new GetVehicleBlueprintByIdQuery(blueprint.Id), TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(blueprint.Id, result.Id);
        Assert.Equal("Honda", result.Brand);
        Assert.Equal("Civic", result.Model);
        Assert.Equal(2, result.PlanTemplates.Count);
    }

    [Fact]
    public async Task Throws_NotFoundException_When_Not_Found()
    {
        var (blueprintRead, _) = new VehicleBlueprintRepositoryBuilder().SetupGetById(null).Build();

        var handler = new GetVehicleBlueprintByIdQueryHandler(blueprintRead);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetVehicleBlueprintByIdQuery(Guid.NewGuid()), TestContext.Current.CancellationToken));
    }
}
