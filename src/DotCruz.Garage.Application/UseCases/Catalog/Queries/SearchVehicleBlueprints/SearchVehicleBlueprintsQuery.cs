using DotCruz.Garage.Application.DTOs.Catalog;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Catalog.Queries.SearchVehicleBlueprints;

public record SearchVehicleBlueprintsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? Brand = null,
    string? Model = null,
    int? Year = null) : IRequest<SearchVehicleBlueprintsResponseDto>;
