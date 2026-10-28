using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Services.Storage;
using DotCruz.Garage.Application.DTOs.Storage;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetUploadUrl;

public class GetUploadUrlQueryHandler(
    IVehicleReadRepository vehicleReadRepository,
    IStorageService storageService,
    IWorkspaceProvider workspaceProvider)
    : IRequestHandler<GetUploadUrlQuery, StorageUploadUrlDto>
{
    public async Task<StorageUploadUrlDto> Handle(GetUploadUrlQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await vehicleReadRepository.GetByIdAsync(request.VehicleId, cancellationToken);
        if (vehicle == null)
        {
            throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);
        }

        var workspaceId = workspaceProvider.WorkspaceId;
        if (!workspaceId.HasValue || !vehicle.CanWrite(workspaceId.Value))
        {
            throw new ForbiddenException(ResourceMessagesException.VEHICLE_ACCESS_DENIED);
        }

        return await storageService.GeneratePresignedUploadUrlAsync(
            workspaceId.Value,
            request.VehicleId,
            request.FileName,
            request.ContentType,
            cancellationToken);
    }
}
