using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.RevokeVehicleShare;

public record RevokeVehicleShareCommand(Guid VehicleId, Guid TargetWorkspaceId) : IRequest;
