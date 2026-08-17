using DotCruz.Garage.Application.DTOs.Accessories;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Accessories.Queries.SearchVehicleAccessories;

public sealed record SearchVehicleAccessoriesQuery(
    Guid? VehicleId = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<SearchVehicleAccessoriesResponseDto>;
