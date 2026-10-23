using MediatR;

namespace DotCruz.Garage.Application.UseCases.Blueprints.Commands.ProcessPendingBlueprints;

public sealed record ProcessPendingBlueprintsCommand(
    int Limit = 5
) : IRequest<int>;
