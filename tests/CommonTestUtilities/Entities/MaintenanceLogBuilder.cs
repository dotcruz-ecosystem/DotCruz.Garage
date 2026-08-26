using Bogus;
using DotCruz.Garage.Domain.Entities.Maintenance;

namespace CommonTestUtilities.Entities;

public class MaintenanceLogBuilder
{
    public static MaintenanceLog Build(
        Guid? tenantId = null,
        Guid? userId = null,
        Guid? vehicleId = null,
        Guid? scheduleId = null,
        int odometerAtService = 50_000,
        decimal cost = 350.00m,
        DateTimeOffset? performedAt = null)
    {
        var faker = new Faker();

        return MaintenanceLog.Create(
            tenantId: tenantId ?? Guid.NewGuid(),
            userId: userId ?? Guid.NewGuid(),
            vehicleId: vehicleId ?? Guid.NewGuid(),
            maintenanceScheduleId: scheduleId ?? Guid.NewGuid(),
            description: faker.Commerce.ProductName(),
            performedAt: performedAt ?? DateTimeOffset.UtcNow,
            odometerAtService: odometerAtService,
            cost: cost,
            receiptUrl: faker.Internet.Url(),
            notes: faker.Lorem.Sentence()
        );
    }
}
