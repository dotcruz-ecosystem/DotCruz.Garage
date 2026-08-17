using DotCruz.Garage.Application.DTOs.Fines;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Fines.Queries.SearchTrafficFines;

public sealed record SearchTrafficFinesQuery(
    Guid? VehicleId = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<SearchTrafficFinesResponseDto>;
