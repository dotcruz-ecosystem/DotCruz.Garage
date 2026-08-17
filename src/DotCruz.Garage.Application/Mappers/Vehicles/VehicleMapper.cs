using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;
using DotCruz.Garage.Domain.Entities.Vehicles;

namespace DotCruz.Garage.Application.Mappers.Vehicles;

public static class VehicleMapper
{
    public static Vehicle ToDomain(this CreateVehicleCommand command, Guid tenantId, Guid userId)
    {
        return Vehicle.Create(
            tenantId,
            userId,
            command.Name,
            command.Brand,
            command.Model,
            command.Year,
            command.Plate,
            command.Vin,
            command.InitialOdometer,
            command.InitialDailyMileageAverage
        );
    }

    public static VehicleDto ToDto(this Vehicle vehicle)
    {
        return new VehicleDto(
            vehicle.Id,
            vehicle.Name,
            vehicle.Brand,
            vehicle.Model,
            vehicle.Year,
            vehicle.Plate,
            vehicle.Vin,
            vehicle.CurrentOdometer,
            vehicle.DailyMileageAverage,
            vehicle.OdometerUpdatedAt
        );
    }
}
