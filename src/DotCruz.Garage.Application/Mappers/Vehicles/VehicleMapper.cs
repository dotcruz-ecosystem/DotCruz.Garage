using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;
using DotCruz.Garage.Domain.Entities.Vehicles;

namespace DotCruz.Garage.Application.Mappers.Vehicles;

public static class VehicleMapper
{
    public static Vehicle ToDomain(this CreateVehicleCommand command, Guid workspaceId, Guid userId)
    {
        var resolvedName = string.IsNullOrWhiteSpace(command.Name)
            ? $"{command.Model.Trim()} {command.Engine.Trim()} - {command.Brand.Trim()} - {command.Year}"
            : command.Name.Trim();

        return Vehicle.Create(
            workspaceId,
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
    }

    public static VehicleDto ToDto(this Vehicle vehicle)
    {
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
            vehicle.OdometerUpdatedAt
        );
    }
}
