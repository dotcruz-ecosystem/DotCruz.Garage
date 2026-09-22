using Bogus;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;

namespace CommonTestUtilities.Requests;

public class CreateVehicleCommandBuilder
{
    public static CreateVehicleCommand Build(
        string? name = null,
        string? brand = null,
        string? model = null,
        int? year = null,
        string? plate = null,
        string? vin = null,
        int? initialOdometer = null,
        double? initialDailyMileageAverage = null,
        bool withoutPlate = false,
        bool withoutVin = false)
    {
        var faker = new Faker();

        return new CreateVehicleCommand(
            Name: name,
            Brand: brand ?? faker.Vehicle.Manufacturer(),
            Model: model ?? faker.Vehicle.Model(),
            Year: year ?? faker.Random.Int(2010, DateTime.UtcNow.Year),
            Plate: withoutPlate ? null : (plate ?? faker.Random.Replace("???####")),
            Vin: withoutVin ? null : (vin ?? faker.Vehicle.Vin()),
            InitialOdometer: initialOdometer ?? faker.Random.Int(0, 100_000),
            InitialDailyMileageAverage: initialDailyMileageAverage ?? 30
        );
    }
}
