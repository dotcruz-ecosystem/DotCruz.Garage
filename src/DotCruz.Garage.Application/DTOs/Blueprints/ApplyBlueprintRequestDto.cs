namespace DotCruz.Garage.Application.DTOs.Blueprints;

public sealed record ApplyBlueprintRequestDto(
    IReadOnlyCollection<ApplyBlueprintItemDto> Items
);

public sealed record ApplyBlueprintItemDto(
    Guid TemplateId,
    string CalculationMode, // "NeverPerformed" | "RecentlyPerformed" | "PendingFirstExecution"
    int? LastPerformedOdometer,
    DateTimeOffset? LastPerformedDate
);
