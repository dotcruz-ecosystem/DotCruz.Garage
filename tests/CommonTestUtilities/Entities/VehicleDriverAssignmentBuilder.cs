using Bogus;
using DotCruz.Garage.Domain.Entities.Drivers;

namespace CommonTestUtilities.Entities;

public class VehicleDriverAssignmentBuilder
{
    public static VehicleDriverAssignment Build(
        Guid? tenantId = null,
        Guid? userId = null,
        Guid? vehicleId = null,
        Guid? driverId = null,
        DateTimeOffset? startedAt = null,
        string? notes = null)
    {
        var faker = new Faker();

        return VehicleDriverAssignment.Create(
            tenantId: tenantId ?? Guid.NewGuid(),
            userId: userId ?? Guid.NewGuid(),
            vehicleId: vehicleId ?? Guid.NewGuid(),
            driverId: driverId ?? Guid.NewGuid(),
            startedAt: startedAt ?? DateTimeOffset.UtcNow,
            notes: notes ?? faker.Lorem.Sentence()
        );
    }
}
