using DotCruz.Garage.Application.DTOs.Documents;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Documents.Queries.SearchVehicleDocuments;

public sealed record SearchVehicleDocumentsQuery(
    Guid? VehicleId = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<SearchVehicleDocumentsResponseDto>;
