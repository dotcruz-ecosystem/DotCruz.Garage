using Bogus;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;
using DotCruz.Garage.Domain.Enums;

namespace CommonTestUtilities.Requests;

public class CreateVehicleCommandBuilder
{
    public static CreateVehicleCommand Build(
        string? name = null,
        VehicleType? type = null,
        string? brand = null,
        string? model = null,
        string? engine = null,
        int? year = null,
        string? plate = null,
        int? initialOdometer = null,
        double? initialDailyMileageAverage = null,
        bool withoutPlate = false)
    {
        var faker = new Faker();

        return new CreateVehicleCommand(
            Name: name,
            Type: type ?? VehicleType.Car,
            Brand: brand ?? faker.Vehicle.Manufacturer(),
            Model: model ?? faker.Vehicle.Model(),
            Engine: engine ?? "2.0",
            Year: year ?? faker.Random.Int(2010, DateTime.UtcNow.Year),
            Plate: withoutPlate ? null : (plate ?? faker.Random.Replace("???####")),
            InitialOdometer: initialOdometer ?? faker.Random.Int(0, 100_000),
            InitialDailyMileageAverage: initialDailyMileageAverage ?? 30
        );
    }
}
