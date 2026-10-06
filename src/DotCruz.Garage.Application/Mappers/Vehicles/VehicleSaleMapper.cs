using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;

namespace DotCruz.Garage.Application.Mappers.Vehicles;

public static class VehicleSaleMapper
{
    public static VehicleSaleDto ToDto(this VehicleSale sale, int? currentOdometer = null, DateTimeOffset? now = null)
    {
        var referenceNow = now ?? DateTimeOffset.UtcNow;
        var odo = currentOdometer ?? sale.Vehicle?.CurrentOdometer ?? sale.SoldOdometer;

        return new VehicleSaleDto(
            sale.Id,
            sale.VehicleId,
            sale.Vehicle?.Name ?? string.Empty,
            sale.CustomerName,
            sale.CustomerEmail,
            sale.CustomerPhone,
            sale.CustomerDocument,
            sale.SoldAt,
            sale.SoldPrice,
            sale.SoldOdometer,
            sale.WarrantyExpiresAt,
            sale.WarrantyKm,
            sale.IsWarrantyActive(odo, referenceNow),
            sale.RemainingWarrantyDays(referenceNow),
            sale.RemainingWarrantyKm(odo),
            sale.Notes,
            sale.ClaimedByUserId,
            sale.ClaimedAt,
            sale.IsActive
        );
    }
}
