using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.TransferVehicleOwnership;

public class TransferVehicleOwnershipCommandHandler(
    IVehicleWriteRepository vehicleWriteRepository,
    IWorkspaceProvider workspaceProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<TransferVehicleOwnershipCommand>
{
    public async Task Handle(TransferVehicleOwnershipCommand request, CancellationToken cancellationToken)
    {
        var currentWorkspaceId = workspaceProvider.WorkspaceId 
            ?? throw new ForbiddenException(ResourceMessagesException.VEHICLE_ACCESS_DENIED);

        if (currentWorkspaceId == request.RequestDto.NewOwnerWorkspaceId)
            throw new ErrorOnValidationException(ResourceMessagesException.CANNOT_TRANSFER_TO_SAME_WORKSPACE);

        var vehicle = await vehicleWriteRepository.GetByIdToUpdateAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        if (!vehicle.IsOwner(currentWorkspaceId))
            throw new ForbiddenException(ResourceMessagesException.VEHICLE_OWNER_REQUIRED);

        vehicle.TransferOwnership(request.RequestDto.NewOwnerWorkspaceId);

        await unitOfWork.CommitAsync(cancellationToken);
    }
}
