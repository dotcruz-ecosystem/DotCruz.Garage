using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.ShareVehicle;

public class ShareVehicleCommandHandler(
    IVehicleWriteRepository vehicleWriteRepository,
    IWorkspaceProvider workspaceProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ShareVehicleCommand, VehicleShareDto>
{
    public async Task<VehicleShareDto> Handle(ShareVehicleCommand request, CancellationToken cancellationToken)
    {
        var currentWorkspaceId = workspaceProvider.WorkspaceId 
            ?? throw new ForbiddenException(ResourceMessagesException.VEHICLE_ACCESS_DENIED);

        var currentUserId = workspaceProvider.UserId ?? Guid.Empty;

        var vehicle = await vehicleWriteRepository.GetByIdToUpdateAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        if (!vehicle.IsOwner(currentWorkspaceId))
        {
            throw new ForbiddenException(ResourceMessagesException.VEHICLE_OWNER_REQUIRED);
        }

        var link = vehicle.AssignWorkspace(request.RequestDto.TargetWorkspaceId, request.RequestDto.Role, currentUserId);

        await unitOfWork.CommitAsync(cancellationToken);

        return new VehicleShareDto(link.WorkspaceId, link.Role, link.CreatedByUserId, link.CreatedAt);
    }
}
