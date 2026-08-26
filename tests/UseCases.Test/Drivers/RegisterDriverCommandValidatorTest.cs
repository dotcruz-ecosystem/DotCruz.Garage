using DotCruz.Garage.Application.DTOs.Drivers;
using DotCruz.Garage.Application.UseCases.Drivers.Commands.RegisterDriver;
using DotCruz.Garage.Domain.Exceptions.Resources;

namespace UseCases.Test.Drivers;

public class RegisterDriverCommandValidatorTest
{
    [Fact]
    public void Valid_Command_Passes()
    {
        var req = new RegisterDriverRequestDto(
            "Carlos da Silva",
            "123.456.789-00",
            "12345678901",
            "B",
            DateTimeOffset.UtcNow.AddYears(3),
            "carlos@frota.com.br",
            "(11) 98888-7777");

        var command = new RegisterDriverCommand(req);
        var validator = new RegisterDriverCommandValidator();

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Empty_Name_Fails()
    {
        var req = new RegisterDriverRequestDto(
            "",
            "123.456.789-00",
            "12345678901",
            "B",
            DateTimeOffset.UtcNow.AddYears(3),
            "carlos@frota.com.br",
            "(11) 98888-7777");

        var command = new RegisterDriverCommand(req);
        var validator = new RegisterDriverCommandValidator();

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ResourceMessagesException.NAME_EMPTY);
    }
}
