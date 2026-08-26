using DotCruz.Garage.Application.DTOs.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.GenerateOdometerCheckinToken;

public record GenerateOdometerCheckinTokenCommand(
    Guid VehicleId,
    TimeSpan? Validity = null) : IRequest<GenerateOdometerCheckinTokenResponseDto>;
