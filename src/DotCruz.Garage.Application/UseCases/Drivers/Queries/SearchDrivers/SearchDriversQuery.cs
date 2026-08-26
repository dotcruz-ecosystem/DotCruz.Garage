using DotCruz.Garage.Application.DTOs.Drivers;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Queries.SearchDrivers;

public record SearchDriversQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    bool? ActiveOnly = null) : IRequest<SearchDriversResponseDto>;
