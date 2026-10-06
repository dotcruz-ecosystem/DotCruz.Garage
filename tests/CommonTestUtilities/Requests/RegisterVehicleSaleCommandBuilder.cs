using Bogus;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.RegisterVehicleSale;

namespace CommonTestUtilities.Requests;

public class RegisterVehicleSaleCommandBuilder
{
    public static RegisterVehicleSaleCommand Build(
        Guid? vehicleId = null,
        string? customerName = null,
        string? customerEmail = null,
        string? customerPhone = null,
        string? customerDocument = null,
        DateTimeOffset? soldAt = null,
        decimal? soldPrice = 75000m,
        int? soldOdometer = 45000,
        DateTimeOffset? warrantyExpiresAt = null,
        int? warrantyKm = 5000,
        string? notes = null)
    {
        var faker = new Faker();

        return new RegisterVehicleSaleCommand(
            VehicleId: vehicleId ?? Guid.NewGuid(),
            CustomerName: customerName ?? faker.Name.FullName(),
            CustomerEmail: customerEmail ?? faker.Internet.Email(),
            CustomerPhone: customerPhone ?? faker.Phone.PhoneNumber(),
            CustomerDocument: customerDocument ?? faker.Random.Replace("###.###.###-##"),
            SoldAt: soldAt ?? DateTimeOffset.UtcNow,
            SoldPrice: soldPrice,
            SoldOdometer: soldOdometer,
            WarrantyExpiresAt: warrantyExpiresAt ?? DateTimeOffset.UtcNow.AddMonths(3),
            WarrantyKm: warrantyKm,
            Notes: notes ?? "Venda pós-venda garantida"
        );
    }
}
