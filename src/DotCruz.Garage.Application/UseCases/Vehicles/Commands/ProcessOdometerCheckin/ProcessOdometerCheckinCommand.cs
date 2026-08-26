using DotCruz.Garage.Application.DTOs.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.ProcessOdometerCheckin;

public record ProcessOdometerCheckinCommand(
    string Token,
    int NewOdometer,
    DateTimeOffset? RecordedAt = null) : IRequest<OdometerCheckinResultDto>;
