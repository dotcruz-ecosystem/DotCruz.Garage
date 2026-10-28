using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.Vehicles;

public record ShareVehicleRequestDto(Guid TargetWorkspaceId, VehicleWorkspaceRole Role);
