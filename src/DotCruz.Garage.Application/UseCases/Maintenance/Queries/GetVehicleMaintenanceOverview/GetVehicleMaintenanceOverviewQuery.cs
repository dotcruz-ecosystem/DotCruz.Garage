using DotCruz.Garage.Application.DTOs.Maintenance;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetVehicleMaintenanceOverview;

public record GetVehicleMaintenanceOverviewQuery(Guid VehicleId) : IRequest<MaintenanceOverviewDto>;
