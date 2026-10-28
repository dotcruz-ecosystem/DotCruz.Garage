using DotCruz.Garage.Application.DTOs.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.ShareVehicle;

public record ShareVehicleCommand(Guid VehicleId, ShareVehicleRequestDto RequestDto) : IRequest<VehicleShareDto>;
