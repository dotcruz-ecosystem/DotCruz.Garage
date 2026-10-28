using DotCruz.Garage.Application.DTOs.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.TransferVehicleOwnership;

public record TransferVehicleOwnershipCommand(Guid VehicleId, TransferOwnershipRequestDto RequestDto) : IRequest;
