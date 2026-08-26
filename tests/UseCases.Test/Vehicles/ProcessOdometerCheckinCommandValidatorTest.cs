using DotCruz.Garage.Application.UseCases.Vehicles.Commands.ProcessOdometerCheckin;
using DotCruz.Garage.Domain.Exceptions.Resources;

namespace UseCases.Test.Vehicles;

public class ProcessOdometerCheckinCommandValidatorTest
{
    [Fact]
    public void Valid_Command_Passes_Validation()
    {
        var command = new ProcessOdometerCheckinCommand("valid-token", 55_000);
        var validator = new ProcessOdometerCheckinCommandValidator();
        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Empty_Token_Fails_Validation()
    {
        var command = new ProcessOdometerCheckinCommand("", 55_000);
        var validator = new ProcessOdometerCheckinCommandValidator();
        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ResourceMessagesException.TOKEN_REQUIRED);
    }

    [Fact]
    public void Negative_Or_Zero_Odometer_Fails_Validation()
    {
        var command = new ProcessOdometerCheckinCommand("valid-token", 0);
        var validator = new ProcessOdometerCheckinCommandValidator();
        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ResourceMessagesException.ODOMETER_NEGATIVE);
    }
}
