using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.CompleteMaintenanceSchedule;
using DotCruz.Garage.Domain.Exceptions.Resources;

namespace UseCases.Test.Maintenance;

public class CompleteMaintenanceScheduleCommandValidatorTest
{
    [Fact]
    public void Valid_Command_Passes()
    {
        var req = new CompleteMaintenanceScheduleRequestDto(DateTimeOffset.UtcNow, 50_000, 200m);
        var command = new CompleteMaintenanceScheduleCommand(Guid.NewGuid(), Guid.NewGuid(), req);
        var validator = new CompleteMaintenanceScheduleCommandValidator();

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Negative_Cost_Fails()
    {
        var req = new CompleteMaintenanceScheduleRequestDto(DateTimeOffset.UtcNow, 50_000, -50m);
        var command = new CompleteMaintenanceScheduleCommand(Guid.NewGuid(), Guid.NewGuid(), req);
        var validator = new CompleteMaintenanceScheduleCommandValidator();

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ResourceMessagesException.COST_NEGATIVE);
    }
}
