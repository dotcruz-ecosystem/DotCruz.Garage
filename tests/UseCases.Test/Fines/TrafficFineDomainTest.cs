using CommonTestUtilities.Entities;
using DotCruz.Garage.Domain.Enums;

namespace UseCases.Test.Fines;

public class TrafficFineDomainTest
{
    [Fact]
    public void IsNICRisk_Returns_True_When_Deadline_Is_Within_15_Days_And_Not_Indicated()
    {
        var fine = TrafficFineBuilder.Build(
            driverId: null,
            indicationDeadline: DateTimeOffset.UtcNow.AddDays(7),
            status: FineStatus.Pending);

        Assert.True(fine.IsNICRisk());
        Assert.True(fine.DaysUntilIndicationDeadline() <= 15);
    }

    [Fact]
    public void IsNICRisk_Returns_False_When_Driver_Already_Indicated()
    {
        var driverId = Guid.NewGuid();
        var fine = TrafficFineBuilder.Build(
            driverId: null,
            indicationDeadline: DateTimeOffset.UtcNow.AddDays(5),
            status: FineStatus.Pending);

        fine.IndicateDriver(driverId, "PROT-998877");

        Assert.False(fine.IsNICRisk());
        Assert.Equal(FineStatus.Indicated, fine.Status);
        Assert.Equal(driverId, fine.DriverId);
        Assert.Equal("PROT-998877", fine.IndicationProtocol);
    }
}
