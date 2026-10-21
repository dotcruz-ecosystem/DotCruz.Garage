using DotCruz.Garage.Application.DTOs.Storage;

namespace DotCruz.Garage.Application.Abstractions.Services.Storage;

public interface IStorageService
{
    Task<StorageUploadUrlDto> GeneratePresignedUploadUrlAsync(
        Guid workspaceId,
        Guid vehicleId,
        string fileName,
        string contentType,
        CancellationToken cancellationToken);
}
