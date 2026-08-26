using Bogus;
using DotCruz.Garage.Domain.Entities.Catalog;
using DotCruz.Garage.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class VehicleBlueprintBuilder
{
    public static VehicleBlueprint Build(
        string? brand = null,
        string? model = null,
        int year = 2023,
        string? engine = "1.3 Firefly",
        string? version = "Drive",
        BlueprintSource source = BlueprintSource.AiExtraction,
        int templatesCount = 3)
    {
        var faker = new Faker();

        var blueprint = VehicleBlueprint.Create(
            brand: brand ?? faker.Vehicle.Manufacturer(),
            model: model ?? faker.Vehicle.Model(),
            year: year,
            engine: engine,
            version: version,
            source: source
        );

        for (int i = 1; i <= templatesCount; i++)
        {
            var template = MaintenancePlanTemplateBuilder.Build(
                blueprintId: blueprint.Id,
                intervalOdometer: i * 10_000,
                intervalTimeMonths: i * 12,
                description: $"Revisão {i * 10_000} km / {i * 12} meses"
            );
            blueprint.AddPlanTemplate(template);
        }

        return blueprint;
    }
}
