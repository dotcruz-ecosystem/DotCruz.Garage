using Bogus;
using DotCruz.Garage.Domain.Entities.Catalog;
using DotCruz.Garage.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class MaintenancePlanTemplateBuilder
{
    public static MaintenancePlanTemplate Build(
        Guid? blueprintId = null,
        string? description = null,
        MaintenanceCategory category = MaintenanceCategory.Engine,
        int? intervalOdometer = 10_000,
        int? intervalTimeMonths = 12,
        List<string>? items = null,
        bool isRecurring = true)
    {
        var faker = new Faker();

        return MaintenancePlanTemplate.Create(
            vehicleBlueprintId: blueprintId ?? Guid.NewGuid(),
            description: description ?? faker.Lorem.Sentence(4),
            category: category,
            intervalOdometer: intervalOdometer,
            intervalTimeMonths: intervalTimeMonths,
            items: items ?? new List<string> { "Troca de óleo", "Filtro de óleo", "Inspeção geral" },
            isRecurring: isRecurring
        );
    }
}
