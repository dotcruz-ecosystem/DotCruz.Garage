using CommonTestUtilities.Entities;
using DotCruz.Garage.Domain.Entities.Drivers;

namespace UseCases.Test.Drivers;

public class DriverDomainTest
{
    [Fact]
    public void IsCnhExpired_Returns_True_When_Date_Is_In_The_Past()
    {
        var driver = DriverBuilder.Build(cnhExpirationDate: DateTimeOffset.UtcNow.AddDays(-1));
        Assert.True(driver.IsCnhExpired());
    }

    [Fact]
    public void IsCnhExpired_Returns_False_When_Date_Is_In_The_Future()
    {
        var driver = DriverBuilder.Build(cnhExpirationDate: DateTimeOffset.UtcNow.AddYears(1));
        Assert.False(driver.IsCnhExpired());
    }

    [Fact]
    public void CalculateAccumulatedPoints_Sums_Only_Last_12_Months()
    {
        var driver = DriverBuilder.Build();
        var fineRecent1 = TrafficFineBuilder.Build(driverId: driver.Id, points: 5, infractionDate: DateTimeOffset.UtcNow.AddMonths(-2));
        var fineRecent2 = TrafficFineBuilder.Build(driverId: driver.Id, points: 4, infractionDate: DateTimeOffset.UtcNow.AddMonths(-6));
        var fineOld = TrafficFineBuilder.Build(driverId: driver.Id, points: 7, infractionDate: DateTimeOffset.UtcNow.AddMonths(-14));

        driver.Fines.Add(fineRecent1);
        driver.Fines.Add(fineRecent2);
        driver.Fines.Add(fineOld);

        var totalPoints = driver.CalculateAccumulatedPoints();
        Assert.Equal(9, totalPoints);
    }

    [Fact]
    public void IsSuspensionRisk_Returns_True_When_Accumulated_Points_Reach_15()
    {
        var driver = DriverBuilder.Build();
        var fine1 = TrafficFineBuilder.Build(driverId: driver.Id, points: 7, infractionDate: DateTimeOffset.UtcNow.AddMonths(-1));
        var fine2 = TrafficFineBuilder.Build(driverId: driver.Id, points: 5, infractionDate: DateTimeOffset.UtcNow.AddMonths(-2));
        var fine3 = TrafficFineBuilder.Build(driverId: driver.Id, points: 4, infractionDate: DateTimeOffset.UtcNow.AddMonths(-3));

        driver.Fines.Add(fine1);
        driver.Fines.Add(fine2);
        driver.Fines.Add(fine3);

        Assert.Equal(16, driver.CalculateAccumulatedPoints());
        Assert.True(driver.IsSuspensionRisk());
    }
}
