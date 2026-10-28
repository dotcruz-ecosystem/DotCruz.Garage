using Bogus;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class VehicleBuilder
{
    public static Vehicle Build(
        Guid? workspaceId = null,
        Guid? userId = null,
        string? name = null,
        VehicleType? type = null,
        string? brand = null,
        string? model = null,
        string? engine = null,
        string? plate = null,
        int initialOdometer = 50_000)
    {
        var faker = new Faker();
        var targetWorkspaceId = workspaceId ?? Guid.NewGuid();
        var targetUserId = userId ?? Guid.NewGuid();

        var vehicle = Vehicle.Create(
            userId: targetUserId,
            name: name ?? faker.Vehicle.Model(),
            type: type ?? VehicleType.Car,
            brand: brand ?? faker.Vehicle.Manufacturer(),
            model: model ?? faker.Vehicle.Model(),
            engine: engine ?? "2.0",
            year: 2020,
            plate: plate ?? faker.Random.Replace("???####"),
            initialOdometer: initialOdometer,
            initialDailyMileageAverage: 30
        );

        vehicle.AssignWorkspace(targetWorkspaceId, VehicleWorkspaceRole.Owner, targetUserId);

        return vehicle;
    }
}
