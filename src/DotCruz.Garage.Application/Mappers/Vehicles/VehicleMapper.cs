using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.Mappers.Vehicles;

public static class VehicleMapper
{
    public static Vehicle ToDomain(this CreateVehicleCommand command, Guid workspaceId, Guid userId)
    {
        var resolvedName = string.IsNullOrWhiteSpace(command.Name)
            ? $"{command.Model.Trim()} {command.Engine.Trim()} - {command.Brand.Trim()} - {command.Year}"
            : command.Name.Trim();

        var vehicle = Vehicle.Create(
            userId,
            resolvedName,
            command.Type,
            command.Brand.Trim(),
            command.Model.Trim(),
            command.Engine.Trim(),
            command.Year,
            command.Plate?.Trim().ToUpper(),
            command.InitialOdometer,
            command.InitialDailyMileageAverage
        );

        vehicle.AssignWorkspace(workspaceId, VehicleWorkspaceRole.Owner, userId);

        return vehicle;
    }

    public static VehicleDto ToDto(this Vehicle vehicle, Guid? currentWorkspaceId = null)
    {
        var role = currentWorkspaceId.HasValue
            ? vehicle.GetRoleInWorkspace(currentWorkspaceId.Value)
            : vehicle.Workspaces.FirstOrDefault()?.Role ?? VehicleWorkspaceRole.Owner;

        var isOwner = role == VehicleWorkspaceRole.Owner;

        return new VehicleDto(
            vehicle.Id,
            vehicle.Name,
            vehicle.Type,
            vehicle.Brand,
            vehicle.Model,
            vehicle.Engine,
            vehicle.Year,
            vehicle.Plate,
            vehicle.CurrentOdometer,
            vehicle.DailyMileageAverage,
            vehicle.OdometerUpdatedAt,
            role?.ToString(),
            isOwner
        );
    }
}
