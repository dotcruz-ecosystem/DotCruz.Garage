using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Reports;
using DotCruz.Garage.Application.UseCases.Reports.Queries.GetFleetTcoSummary;
using DotCruz.Garage.Application.UseCases.Reports.Queries.GetVehiclePassport;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Reports;

public class ReportsController(IMediator mediator) : GarageBaseController
{
    [HttpGet]
    [Route("tco")]
    [ProducesResponseType(typeof(FleetTcoSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTco(
        [FromQuery] DateTimeOffset? startDate = null,
        [FromQuery] DateTimeOffset? endDate = null,
        [FromQuery] Guid? vehicleId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFleetTcoSummaryQuery(startDate, endDate, vehicleId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("~/api/vehicles/{vehicleId:guid}/passport")]
    [ProducesResponseType(typeof(VehiclePassportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPassport(
        [FromRoute] Guid vehicleId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetVehiclePassportQuery(vehicleId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
