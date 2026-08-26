using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Drivers.Queries.GetDriverFinesSummary;

namespace UseCases.Test.Drivers;

public class GetDriverFinesSummaryQueryHandlerTest
{
    [Fact]
    public async Task Returns_Summary_With_Total_Points_And_Risk()
    {
        var driver = DriverBuilder.Build();
        var fine1 = TrafficFineBuilder.Build(driverId: driver.Id, points: 7, infractionDate: DateTimeOffset.UtcNow.AddMonths(-2), amount: 293.47m);
        var fine2 = TrafficFineBuilder.Build(driverId: driver.Id, points: 5, infractionDate: DateTimeOffset.UtcNow.AddMonths(-1), amount: 195.23m);
        var fine3 = TrafficFineBuilder.Build(driverId: driver.Id, points: 4, infractionDate: DateTimeOffset.UtcNow.AddMonths(-3), amount: 130.16m);

        driver.Fines.Add(fine1);
        driver.Fines.Add(fine2);
        driver.Fines.Add(fine3);

        var (driverRead, _) = new DriverRepositoryBuilder()
            .SetupGetByIdWithFines(driver)
            .Build();

        var handler = new GetDriverFinesSummaryQueryHandler(driverRead);

        var query = new GetDriverFinesSummaryQuery(driver.Id);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(driver.Id, result.DriverId);
        Assert.Equal(16, result.TotalPointsLast12Months);
        Assert.True(result.IsSuspensionRisk);
        Assert.Equal(3, result.TotalFinesCount);
        Assert.Equal(618.86m, result.TotalFinesAmount);
    }
}
