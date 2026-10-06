using DotCruz.Garage.Application.DTOs.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.SubmitPostSaleOdometer;

public sealed record SubmitPostSaleOdometerCommand(
    string Token,
    int NewOdometer,
    DateTimeOffset? RecordedAt = null
) : IRequest<OdometerCheckinResultDto>;
