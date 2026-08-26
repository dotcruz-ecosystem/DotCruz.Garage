using MediatR;

namespace DotCruz.Garage.Application.UseCases.Catalog.Commands.ApplyBlueprintToVehicle;

public record ApplyBlueprintToVehicleCommand(Guid VehicleId, Guid BlueprintId) : IRequest<int>;
