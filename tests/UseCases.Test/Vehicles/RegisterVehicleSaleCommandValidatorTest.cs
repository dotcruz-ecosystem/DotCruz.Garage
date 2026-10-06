using CommonTestUtilities.Requests;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.RegisterVehicleSale;

namespace UseCases.Test.Vehicles;

public class RegisterVehicleSaleCommandValidatorTest
{
    private readonly RegisterVehicleSaleCommandValidator _validator = new();

    [Fact]
    public void Successfully_Validates_Valid_Command()
    {
        var command = RegisterVehicleSaleCommandBuilder.Build();
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Fails_When_VehicleId_Is_Empty()
    {
        var command = RegisterVehicleSaleCommandBuilder.Build(vehicleId: Guid.Empty);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.VehicleId));
    }

    [Fact]
    public void Fails_When_CustomerName_Is_Empty()
    {
        var command = RegisterVehicleSaleCommandBuilder.Build(customerName: "");
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.CustomerName));
    }

    [Fact]
    public void Fails_When_CustomerEmail_Is_Invalid()
    {
        var command = RegisterVehicleSaleCommandBuilder.Build(customerEmail: "not-an-email");
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.CustomerEmail));
    }

    [Fact]
    public void Fails_When_SoldOdometer_Is_Negative()
    {
        var command = RegisterVehicleSaleCommandBuilder.Build(soldOdometer: -100);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.SoldOdometer));
    }

    [Fact]
    public void Fails_When_WarrantyKm_Is_Zero_Or_Negative()
    {
        var command = RegisterVehicleSaleCommandBuilder.Build(warrantyKm: 0);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.WarrantyKm));
    }

    [Fact]
    public void Fails_When_SoldPrice_Is_Zero_Or_Negative()
    {
        var command = RegisterVehicleSaleCommandBuilder.Build(soldPrice: -500m);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.SoldPrice));
    }
}
