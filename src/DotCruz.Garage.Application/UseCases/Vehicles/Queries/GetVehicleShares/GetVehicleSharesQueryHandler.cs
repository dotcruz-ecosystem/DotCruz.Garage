using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetVehicleShares;

public class GetVehicleSharesQueryHandler(
    IVehicleReadRepository vehicleReadRepository,
    IWorkspaceProvider workspaceProvider)
    : IRequestHandler<GetVehicleSharesQuery, IReadOnlyCollection<VehicleShareDto>>
{
    public async Task<IReadOnlyCollection<VehicleShareDto>> Handle(GetVehicleSharesQuery request, CancellationToken cancellationToken)
    {
        var currentWorkspaceId = workspaceProvider.WorkspaceId 
            ?? throw new ForbiddenException(ResourceMessagesException.VEHICLE_ACCESS_DENIED);

        var vehicle = await vehicleReadRepository.GetByIdAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        if (!vehicle.HasAccess(currentWorkspaceId, VehicleWorkspaceRole.Viewer))
        {
            throw new ForbiddenException(ResourceMessagesException.VEHICLE_ACCESS_DENIED);
        }

        return vehicle.Workspaces
            .Where(w => w.DeletedAt == null)
            .Select(w => new VehicleShareDto(w.WorkspaceId, w.Role, w.CreatedByUserId, w.CreatedAt))
            .ToList();
    }
}
