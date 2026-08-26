using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Drivers;
using DotCruz.Garage.Application.Mappers.Drivers;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Queries.SearchDrivers;

public class SearchDriversQueryHandler : IRequestHandler<SearchDriversQuery, SearchDriversResponseDto>
{
    private readonly IDriverReadRepository _driverReadRepository;

    public SearchDriversQueryHandler(IDriverReadRepository driverReadRepository)
    {
        _driverReadRepository = driverReadRepository;
    }

    public async Task<SearchDriversResponseDto> Handle(SearchDriversQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _driverReadRepository.SearchAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.ActiveOnly,
            cancellationToken);

        var dtos = items.Select(d => d.ToDto()).ToList();
        return new SearchDriversResponseDto(dtos, request.PageNumber, request.PageSize, totalCount);
    }
}
