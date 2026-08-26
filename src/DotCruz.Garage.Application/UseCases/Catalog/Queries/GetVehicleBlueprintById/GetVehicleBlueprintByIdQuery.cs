using DotCruz.Garage.Application.DTOs.Catalog;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Catalog.Queries.GetVehicleBlueprintById;

public record GetVehicleBlueprintByIdQuery(Guid Id) : IRequest<VehicleBlueprintDto>;
