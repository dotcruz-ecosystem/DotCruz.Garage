using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.RevokeVehicleShare;

public class RevokeVehicleShareCommandHandler(
    IVehicleWriteRepository vehicleWriteRepository,
    IWorkspaceProvider workspaceProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RevokeVehicleShareCommand>
{
    public async Task Handle(RevokeVehicleShareCommand request, CancellationToken cancellationToken)
    {
        var currentWorkspaceId = workspaceProvider.WorkspaceId 
            ?? throw new ForbiddenException(ResourceMessagesException.VEHICLE_ACCESS_DENIED);

        var vehicle = await vehicleWriteRepository.GetByIdToUpdateAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        if (!vehicle.IsOwner(currentWorkspaceId))
        {
            throw new ForbiddenException(ResourceMessagesException.VEHICLE_OWNER_REQUIRED);
        }

        var removed = vehicle.RemoveWorkspace(request.TargetWorkspaceId);
        if (!removed)
        {
            throw new NotFoundException(ResourceMessagesException.VEHICLE_SHARE_NOT_FOUND);
        }

        await unitOfWork.CommitAsync(cancellationToken);
    }
}
