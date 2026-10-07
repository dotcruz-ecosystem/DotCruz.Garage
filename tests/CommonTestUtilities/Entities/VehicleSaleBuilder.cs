using Bogus;
using DotCruz.Garage.Domain.Entities.Vehicles;

namespace CommonTestUtilities.Entities;

public class VehicleSaleBuilder
{
    public static VehicleSale Build(
        Guid? workspaceId = null,
        Guid? userId = null,
        Guid? vehicleId = null,
        string? customerName = null,
        string? customerEmail = null,
        string? customerPhone = null,
        string? customerDocument = null,
        DateTimeOffset? soldAt = null,
        decimal? soldPrice = 65000m,
        int soldOdometer = 45000,
        DateTimeOffset? warrantyExpiresAt = null,
        int? warrantyKm = 5000,
        string? notes = null)
    {
        var faker = new Faker();

        return VehicleSale.Create(
            workspaceId: workspaceId ?? Guid.NewGuid(),
            userId: userId ?? Guid.NewGuid(),
            vehicleId: vehicleId ?? Guid.NewGuid(),
            customerName: customerName ?? faker.Name.FullName(),
            customerEmail: customerEmail ?? faker.Internet.Email(),
            customerPhone: customerPhone ?? faker.Phone.PhoneNumber(),
            customerDocument: customerDocument ?? faker.Random.Replace("###.###.###-##"),
            soldAt: soldAt ?? DateTimeOffset.UtcNow.AddDays(-10),
            soldPrice: soldPrice,
            soldOdometer: soldOdometer,
            warrantyExpiresAt: warrantyExpiresAt ?? DateTimeOffset.UtcNow.AddMonths(3),
            warrantyKm: warrantyKm,
            notes: notes ?? "Venda pós-venda garantia estendida"
        );
    }
}
