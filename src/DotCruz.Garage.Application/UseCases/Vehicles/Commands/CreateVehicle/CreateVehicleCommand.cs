using DotCruz.Garage.Domain.Enums;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;

public sealed record CreateVehicleCommand(
    string? Name,
    VehicleType Type,
    string Brand,
    string Model,
    string Engine,
    int Year,
    string? Plate,
    int InitialOdometer,
    double InitialDailyMileageAverage
) : IRequest<Guid>;
