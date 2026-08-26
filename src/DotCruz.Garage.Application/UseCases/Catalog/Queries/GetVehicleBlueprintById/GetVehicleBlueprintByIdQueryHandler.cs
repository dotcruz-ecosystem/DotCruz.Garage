using DotCruz.Garage.Application.Abstractions.Data.Repositories.Catalog;
using DotCruz.Garage.Application.DTOs.Catalog;
using DotCruz.Garage.Application.Mappers.Catalog;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Catalog.Queries.GetVehicleBlueprintById;

public class GetVehicleBlueprintByIdQueryHandler : IRequestHandler<GetVehicleBlueprintByIdQuery, VehicleBlueprintDto>
{
    private readonly IVehicleBlueprintReadRepository _blueprintReadRepository;

    public GetVehicleBlueprintByIdQueryHandler(IVehicleBlueprintReadRepository blueprintReadRepository)
    {
        _blueprintReadRepository = blueprintReadRepository;
    }

    public async Task<VehicleBlueprintDto> Handle(GetVehicleBlueprintByIdQuery request, CancellationToken cancellationToken)
    {
        var blueprint = await _blueprintReadRepository.GetByIdWithTemplatesAsync(request.Id, cancellationToken);
        if (blueprint is null)
        {
            throw new NotFoundException(ResourceMessagesException.BLUEPRINT_NOT_FOUND);
        }

        return blueprint.ToDto();
    }
}
