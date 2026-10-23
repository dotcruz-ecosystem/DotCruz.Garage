using CommonTestUtilities.Requests;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;
using DotCruz.Garage.Domain.Exceptions.Resources;

namespace UseCases.Test.Vehicles;

public class CreateVehicleCommandValidatorTest
{
    [Fact]
    public void Valid_Command_With_Explicit_Name_Passes_Validation()
    {
        var command = CreateVehicleCommandBuilder.Build(
            name: "Van Executiva 01",
            brand: "Mercedes-Benz",
            model: "Sprinter",
            year: 2023,
            plate: "BRA2E19",
            initialOdometer: 10_000,
            initialDailyMileageAverage: 50);

        var validator = new CreateVehicleCommandValidator();
        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Valid_Command_Without_Name_Passes_Validation()
    {
        var command = CreateVehicleCommandBuilder.Build(
            name: null,
            brand: "Toyota",
            model: "Corolla",
            year: 2024,
            plate: null,
            initialOdometer: 0,
            initialDailyMileageAverage: 30);

        var validator = new CreateVehicleCommandValidator();
        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Empty_Brand_Fails_Validation()
    {
        var command = CreateVehicleCommandBuilder.Build(brand: "");

        var validator = new CreateVehicleCommandValidator();
        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ResourceMessagesException.BRAND_EMPTY);
    }

    [Fact]
    public void Empty_Model_Fails_Validation()
    {
        var command = CreateVehicleCommandBuilder.Build(model: "");

        var validator = new CreateVehicleCommandValidator();
        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ResourceMessagesException.MODEL_EMPTY);
    }

    [Fact]
    public void Invalid_Year_Fails_Validation()
    {
        var command = CreateVehicleCommandBuilder.Build(year: 1899);

        var validator = new CreateVehicleCommandValidator();
        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ResourceMessagesException.YEAR_INVALID);
    }

    [Fact]
    public void Negative_Odometer_Fails_Validation()
    {
        var command = CreateVehicleCommandBuilder.Build(initialOdometer: -1);

        var validator = new CreateVehicleCommandValidator();
        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ResourceMessagesException.ODOMETER_NEGATIVE);
    }

    [Fact]
    public void Empty_Engine_Fails_Validation()
    {
        var command = CreateVehicleCommandBuilder.Build(engine: "");

        var validator = new CreateVehicleCommandValidator();
        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ResourceMessagesException.ENGINE_EMPTY);
    }

    [Fact]
    public void Invalid_VehicleType_Fails_Validation()
    {
        var command = CreateVehicleCommandBuilder.Build(type: (DotCruz.Garage.Domain.Enums.VehicleType)999);

        var validator = new CreateVehicleCommandValidator();
        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ResourceMessagesException.VEHICLE_TYPE_INVALID);
    }
}
