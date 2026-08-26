using DotCruz.Garage.Application.DTOs.Reports;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Reports.Queries.GetFleetTcoSummary;

public record GetFleetTcoSummaryQuery(
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null,
    Guid? VehicleId = null) : IRequest<FleetTcoSummaryDto>;
