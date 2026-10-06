using Bogus;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class MaintenanceScheduleBuilder
{
    public static MaintenanceSchedule Build(
        Guid? tenantId = null,
        Guid? userId = null,
        Guid? vehicleId = null,
        string? description = null,
        int currentOdometer = 50_000,
        int? intervalOdometer = 10_000,
        int? lastPerformedOdometer = 45_000,
        int? warningThresholdOdometer = 9_000)
    {
        var faker = new Faker();

        return MaintenanceSchedule.Create(
            tenantId: tenantId ?? Guid.NewGuid(),
            userId: userId ?? Guid.NewGuid(),
            vehicleId: vehicleId ?? Guid.NewGuid(),
            description: description ?? faker.Lorem.Sentence(3),
            category: MaintenanceCategory.General,
            intervalOdometer: intervalOdometer,
            intervalTimeMonths: null,
            lastPerformedOdometer: lastPerformedOdometer,
            lastPerformedDate: null,
            warningThresholdOdometer: warningThresholdOdometer,
            warningThresholdDays: null,
            currentOdometer: currentOdometer,
            dailyMileageAverage: 30
        );
    }
}
