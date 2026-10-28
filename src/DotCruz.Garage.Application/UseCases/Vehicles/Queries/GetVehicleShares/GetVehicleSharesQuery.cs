using DotCruz.Garage.Application.DTOs.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetVehicleShares;

public record GetVehicleSharesQuery(Guid VehicleId) : IRequest<IReadOnlyCollection<VehicleShareDto>>;
