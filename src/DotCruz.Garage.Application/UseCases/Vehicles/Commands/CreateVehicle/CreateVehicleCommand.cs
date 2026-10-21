using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;

public sealed record CreateVehicleCommand(
    string? Name,
    string Brand,
    string Model,
    int Year,
    string? Plate,
    int InitialOdometer,
    double InitialDailyMileageAverage
) : IRequest<Guid>;
