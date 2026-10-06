using DotCruz.Garage.Application.DTOs.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetPostSaleVehicles;

public sealed record GetPostSaleVehiclesQuery : IRequest<IReadOnlyCollection<VehicleSaleDto>>;
