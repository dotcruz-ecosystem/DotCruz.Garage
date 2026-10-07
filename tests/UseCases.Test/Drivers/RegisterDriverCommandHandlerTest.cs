using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.DTOs.Drivers;
using DotCruz.Garage.Application.UseCases.Drivers.Commands.RegisterDriver;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.Drivers;

public class RegisterDriverCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Registers_Driver()
    {
        var workspaceId = Guid.NewGuid();
        var (driverRead, driverWrite) = new DriverRepositoryBuilder()
            .SetupGetByCpf(null)
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder().AsWorkspace(workspaceId).Build();

        var handler = new RegisterDriverCommandHandler(
            driverWrite,
            driverRead,
            workspaceProvider,
            UnitOfWorkBuilder.Build());

        var reqDto = new RegisterDriverRequestDto(
            "Carlos da Silva",
            "123.456.789-00",
            "12345678901",
            "B",
            DateTimeOffset.UtcNow.AddYears(3),
            "carlos@frota.com.br",
            "(11) 98888-7777");

        var command = new RegisterDriverCommand(reqDto);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Throws_ErrorOnValidationException_When_Cpf_Already_Exists()
    {
        var existingDriver = DriverBuilder.Build(cpf: "123.456.789-00");
        var (driverRead, driverWrite) = new DriverRepositoryBuilder()
            .SetupGetByCpf(existingDriver)
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder().Build();

        var handler = new RegisterDriverCommandHandler(
            driverWrite,
            driverRead,
            workspaceProvider,
            UnitOfWorkBuilder.Build());

        var reqDto = new RegisterDriverRequestDto(
            "Outro Carlos",
            "123.456.789-00",
            "99999999999",
            "B",
            DateTimeOffset.UtcNow.AddYears(2),
            "outro@frota.com.br",
            "(11) 91111-2222");

        var command = new RegisterDriverCommand(reqDto);
        await Assert.ThrowsAsync<ErrorOnValidationException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));
    }
}
