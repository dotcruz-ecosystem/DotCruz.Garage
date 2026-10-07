using Bogus;
using DotCruz.Garage.Domain.Entities.Accessories;

namespace CommonTestUtilities.Entities;

public class VehicleAccessoryBuilder
{
    public static VehicleAccessory Build(
        Guid? workspaceId = null,
        Guid? userId = null,
        Guid? vehicleId = null,
        string name = "Rastreador GPS",
        decimal cost = 450.00m,
        DateTimeOffset? purchaseDate = null)
    {
        var faker = new Faker();

        return VehicleAccessory.Create(
            workspaceId: workspaceId ?? Guid.NewGuid(),
            userId: userId ?? Guid.NewGuid(),
            vehicleId: vehicleId ?? Guid.NewGuid(),
            name: name,
            store: faker.Company.CompanyName(),
            purchaseDate: purchaseDate ?? DateTimeOffset.UtcNow.AddMonths(-3),
            cost: cost,
            receiptUrl: null,
            notes: faker.Lorem.Sentence()
        );
    }
}
