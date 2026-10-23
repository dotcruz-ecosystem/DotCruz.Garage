using DotCruz.Garage.Domain.Enums;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Blueprints.Commands.RequestVehicleBlueprint;

public sealed record RequestVehicleBlueprintCommand(
    string Brand,
    string Model,
    string Engine,
    VehicleType Type,
    int Year
) : IRequest<Guid>;
