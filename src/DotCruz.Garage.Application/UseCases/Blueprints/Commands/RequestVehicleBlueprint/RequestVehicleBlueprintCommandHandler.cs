using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Blueprints.Commands.RequestVehicleBlueprint;

public class RequestVehicleBlueprintCommandHandler : IRequestHandler<RequestVehicleBlueprintCommand, Guid>
{
    private readonly IVehicleBlueprintRepository _blueprintRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RequestVehicleBlueprintCommandHandler(
        IVehicleBlueprintRepository blueprintRepository,
        IUnitOfWork unitOfWork)
    {
        _blueprintRepository = blueprintRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RequestVehicleBlueprintCommand request, CancellationToken cancellationToken)
    {
        var existing = await _blueprintRepository.FindMatchingBlueprintAsync(
            request.Brand,
            request.Model,
            request.Engine,
            request.Year,
            request.Type,
            cancellationToken);

        if (existing is not null)
        {
            return existing.Id;
        }

        var yearFrom = request.Year;
        var yearTo = request.Year;

        var blueprint = VehicleBlueprint.Create(
            request.Brand,
            request.Model,
            request.Engine,
            request.Type,
            yearFrom,
            yearTo);

        await _blueprintRepository.AddAsync(blueprint, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return blueprint.Id;
    }
}
