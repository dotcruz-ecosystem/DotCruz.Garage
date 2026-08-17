using DotCruz.Garage.Application.DTOs.Maintenance;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Queries.SearchMaintenanceLogs;

public sealed record SearchMaintenanceLogsQuery(
    Guid VehicleId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<SearchMaintenanceLogsResponseDto>;
