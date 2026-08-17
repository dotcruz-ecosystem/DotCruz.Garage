using DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Documents;
using DotCruz.Garage.Application.Mappers.Documents;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Documents.Queries.SearchVehicleDocuments;

public class SearchVehicleDocumentsQueryHandler : IRequestHandler<SearchVehicleDocumentsQuery, SearchVehicleDocumentsResponseDto>
{
    private readonly IVehicleDocumentReadRepository _repository;
    private readonly IVehicleReadRepository _vehicleRepository;

    public SearchVehicleDocumentsQueryHandler(
        IVehicleDocumentReadRepository repository,
        IVehicleReadRepository vehicleRepository)
    {
        _repository = repository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<SearchVehicleDocumentsResponseDto> Handle(SearchVehicleDocumentsQuery request, CancellationToken cancellationToken)
    {
        if (request.VehicleId.HasValue)
        {
            var vehicleExists = await _vehicleRepository.ExistsAsync(request.VehicleId.Value, cancellationToken);
            if (!vehicleExists)
            {
                throw new NotFoundException("Vehicle not found.");
            }
        }

        var (items, totalCount) = await _repository.SearchAsync(
            request.PageNumber,
            request.PageSize,
            request.VehicleId,
            cancellationToken
        );

        var dtos = items.Select(d => d.ToDto()).ToList();

        return new SearchVehicleDocumentsResponseDto(
            dtos,
            request.PageNumber,
            request.PageSize,
            totalCount
        );
    }
}
