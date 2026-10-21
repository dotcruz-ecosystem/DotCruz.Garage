using Bogus;
using DotCruz.Garage.Domain.Entities.Vehicles;

namespace CommonTestUtilities.Entities;

public class VehicleBuilder
{
    public static Vehicle Build(
        Guid? workspaceId = null,
        Guid? userId = null,
        string? name = null,
        string? plate = null,
        int initialOdometer = 50_000)
    {
        var faker = new Faker();

        return Vehicle.Create(
            workspaceId: workspaceId ?? Guid.NewGuid(),
            userId: userId ?? Guid.NewGuid(),
            name: name ?? faker.Vehicle.Model(),
            brand: faker.Vehicle.Manufacturer(),
            model: faker.Vehicle.Model(),
            year: 2020,
            plate: plate ?? faker.Random.Replace("???####"),
            initialOdometer: initialOdometer,
            initialDailyMileageAverage: 30
        );
    }
}
