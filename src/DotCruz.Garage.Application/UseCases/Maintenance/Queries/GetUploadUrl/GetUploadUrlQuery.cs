using DotCruz.Garage.Application.DTOs.Storage;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetUploadUrl;

public record GetUploadUrlQuery(
    Guid VehicleId,
    string FileName,
    string ContentType,
    long FileSize
) : IRequest<StorageUploadUrlDto>;
