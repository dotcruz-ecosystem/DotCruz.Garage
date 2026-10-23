using Bogus;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class VehicleBlueprintBuilder
{
    public static VehicleBlueprint Build(
        string? brand = null,
        string? model = null,
        string? engine = null,
        VehicleType? type = null,
        int yearFrom = 2020,
        int yearTo = 2025,
        string? sourceUrl = null,
        bool withTemplates = true)
    {
        var faker = new Faker();

        var blueprint = VehicleBlueprint.Create(
            brand ?? faker.Vehicle.Manufacturer(),
            model ?? faker.Vehicle.Model(),
            engine ?? "2.0 Flex",
            type ?? VehicleType.Car,
            yearFrom,
            yearTo,
            sourceUrl ?? faker.Internet.Url());

        if (withTemplates)
        {
            blueprint.AddTemplate("Troca de Óleo do Motor e Filtro", MaintenanceCategory.Engine, 10_000, 12, true);
            blueprint.AddTemplate("Troca de Filtro de Combustível", MaintenanceCategory.Engine, 10_000, 12, true);
            blueprint.AddTemplate("Troca do Fluido de Freio", MaintenanceCategory.Brakes, 40_000, 24, true);
            blueprint.MarkAsReady(sourceUrl, yearFrom, yearTo);
        }

        return blueprint;
    }
}
