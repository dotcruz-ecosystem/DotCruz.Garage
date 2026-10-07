using DotCruz.Garage.Application.DTOs.Accessories;
using DotCruz.Garage.Application.UseCases.Accessories.Commands.CreateVehicleAccessory;
using DotCruz.Garage.Domain.Entities.Accessories;

namespace DotCruz.Garage.Application.Mappers.Accessories;

public static class VehicleAccessoryMapper
{
    public static VehicleAccessory ToDomain(
        this CreateVehicleAccessoryCommand command,
        Guid workspaceId,
        Guid userId)
    {
        return VehicleAccessory.Create(
            workspaceId,
            userId,
            command.VehicleId,
            command.RequestDto.Name,
            command.RequestDto.Store,
            command.RequestDto.PurchaseDate,
            command.RequestDto.Cost,
            command.RequestDto.ReceiptUrl,
            command.RequestDto.Notes
        );
    }

    public static VehicleAccessoryDto ToDto(this VehicleAccessory acc)
    {
        return new VehicleAccessoryDto(
            acc.Id,
            acc.VehicleId,
            acc.Name,
            acc.Store,
            acc.PurchaseDate,
            acc.Cost,
            acc.ReceiptUrl,
            acc.Notes
        );
    }
}
