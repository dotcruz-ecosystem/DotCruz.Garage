using DotCruz.Garage.Application.DTOs.Blueprints;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Blueprints.Queries.GetVehicleBlueprintByVehicleId;

public sealed record GetVehicleBlueprintByVehicleIdQuery(
    Guid VehicleId
) : IRequest<VehicleBlueprintDto?>;
