using DotCruz.Garage.Application.DTOs.Blueprints;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Blueprints.Commands.ApplyBlueprintToVehicle;

public sealed record ApplyBlueprintToVehicleCommand(
    Guid VehicleId,
    ApplyBlueprintRequestDto RequestDto
) : IRequest<IReadOnlyCollection<Guid>>;
