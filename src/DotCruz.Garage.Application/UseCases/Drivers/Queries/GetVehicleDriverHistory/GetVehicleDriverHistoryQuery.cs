using DotCruz.Garage.Application.DTOs.Drivers;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Queries.GetVehicleDriverHistory;

public record GetVehicleDriverHistoryQuery(Guid VehicleId) : IRequest<IReadOnlyCollection<VehicleDriverAssignmentDto>>;
