using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.Vehicles;

public record VehicleShareDto(
    Guid WorkspaceId,
    VehicleWorkspaceRole Role,
    Guid CreatedByUserId,
    DateTimeOffset CreatedAt
);
