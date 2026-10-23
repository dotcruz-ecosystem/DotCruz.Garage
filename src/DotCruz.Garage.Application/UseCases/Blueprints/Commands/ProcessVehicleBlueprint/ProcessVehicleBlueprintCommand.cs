using MediatR;

namespace DotCruz.Garage.Application.UseCases.Blueprints.Commands.ProcessVehicleBlueprint;

public sealed record ProcessVehicleBlueprintCommand(
    Guid BlueprintId
) : IRequest;
