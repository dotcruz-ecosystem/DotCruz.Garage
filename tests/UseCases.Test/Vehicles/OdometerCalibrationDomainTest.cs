using CommonTestUtilities.Entities;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Services;

namespace UseCases.Test.Vehicles;

public class OdometerCalibrationDomainTest
{
    [Fact]
    public void CalculateDailyAverage_ReturnsFallback_WhenFewerThanTwoPoints()
    {
        var history = new List<OdometerHistory>
        {
            OdometerHistory.Create(Guid.NewGuid(), 50_000, DateTimeOffset.UtcNow)
        };

        var result = OdometerCalculator.CalculateDailyAverage(history, fallbackEstimate: 30);

        Assert.Equal(30, result);
    }

    [Fact]
    public void CalculateDailyAverage_UsesWindow_WhenAtLeastTwoPointsInLast90Days()
    {
        var vehicleId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var history = new List<OdometerHistory>
        {
            OdometerHistory.Create(vehicleId, 40_000, now.AddDays(-180)),
            OdometerHistory.Create(vehicleId, 48_000, now.AddDays(-30)),
            OdometerHistory.Create(vehicleId, 50_000, now)
        };

        // Window has: 48,000 (-30d) and 50,000 (now). Diff = 2,000 / 30 = 66.7
        var result = OdometerCalculator.CalculateDailyAverage(history, fallbackEstimate: 30);

        Assert.Equal(66.7, result);
    }

    [Fact]
    public void CalculateDailyAverage_UsesAdaptiveWideSpan_WhenOnlyOnePointInLast90Days()
    {
        var vehicleId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var history = new List<OdometerHistory>
        {
            OdometerHistory.Create(vehicleId, 30_000, now.AddDays(-200)),
            OdometerHistory.Create(vehicleId, 50_000, now)
        };

        // Total span: 20,000 km in 200 days = 100.0 km/day
        var result = OdometerCalculator.CalculateDailyAverage(history, fallbackEstimate: 30);

        Assert.Equal(100.0, result);
    }

    [Fact]
    public void RecordOdometerReading_AdvancesOdometer_WhenReadingIsHigher()
    {
        var vehicle = VehicleBuilder.Build(initialOdometer: 50_000);
        var date = DateTimeOffset.UtcNow.AddHours(1);

        vehicle.RecordOdometerReading(55_000, date);

        Assert.Equal(55_000, vehicle.CurrentOdometer);
        Assert.Equal(date, vehicle.OdometerUpdatedAt);
        Assert.Equal(2, vehicle.OdometerHistories.Count);
    }

    [Fact]
    public void RecordOdometerReading_KeepsCurrentOdometer_WhenReadingIsLower()
    {
        var vehicle = VehicleBuilder.Build(initialOdometer: 50_000);
        var pastDate = DateTimeOffset.UtcNow.AddDays(-60);

        vehicle.RecordOdometerReading(40_000, pastDate);

        Assert.Equal(50_000, vehicle.CurrentOdometer);
        Assert.Equal(2, vehicle.OdometerHistories.Count);
        // Recalculates: 50,000 - 40,000 = 10,000 in 60 days = 166.7 km/day
        Assert.Equal(166.7, vehicle.DailyMileageAverage);
    }

    [Fact]
    public void RecordOdometerReading_DoesNotDuplicate_WhenSameDayAndOdometer()
    {
        var vehicle = VehicleBuilder.Build(initialOdometer: 50_000);
        var serviceDate = new DateTimeOffset(2026, 10, 15, 12, 0, 0, TimeSpan.Zero);

        var addedFirst = vehicle.RecordOdometerReading(48_000, serviceDate);
        var addedSecond = vehicle.RecordOdometerReading(48_000, serviceDate.AddHours(2));

        Assert.True(addedFirst);
        Assert.False(addedSecond);
        Assert.Equal(2, vehicle.OdometerHistories.Count);
    }
}
