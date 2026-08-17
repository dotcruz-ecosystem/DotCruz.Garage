using DotCruz.Garage.Application.DTOs.Maintenance;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Queries.SearchMaintenanceSchedules;

public sealed record SearchMaintenanceSchedulesQuery(
    Guid VehicleId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<SearchMaintenanceSchedulesResponseDto>;
