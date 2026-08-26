using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.DTOs.Fines;
using DotCruz.Garage.Application.UseCases.Fines.Commands.IndicateFineDriver;
using DotCruz.Garage.Domain.Enums;

namespace UseCases.Test.Fines;

public class IndicateFineDriverCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Indicates_Driver_For_Fine()
    {
        var driver = DriverBuilder.Build();
        var fine = TrafficFineBuilder.Build(
            driverId: null,
            indicationDeadline: DateTimeOffset.UtcNow.AddDays(10),
            status: FineStatus.Pending);

        var (driverRead, _) = new DriverRepositoryBuilder().SetupGetById(driver).Build();
        var (_, fineWrite) = new TrafficFineRepositoryBuilder().SetupGetByIdToUpdate(fine).Build();

        var handler = new IndicateFineDriverCommandHandler(
            fineWrite,
            driverRead,
            UnitOfWorkBuilder.Build());

        var reqDto = new IndicateFineDriverRequestDto(driver.Id, "DETRAN-IND-2026-999");
        var command = new IndicateFineDriverCommand(fine.Id, reqDto);

        await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.Equal(driver.Id, fine.DriverId);
        Assert.Equal("DETRAN-IND-2026-999", fine.IndicationProtocol);
        Assert.Equal(FineStatus.Indicated, fine.Status);
        Assert.NotNull(fine.IndicatedAt);
    }
}
