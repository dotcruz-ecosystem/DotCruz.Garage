using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.CreateMaintenanceSchedule;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.LogMaintenance;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.SearchMaintenanceLogs;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.SearchMaintenanceSchedules;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Maintenances;

public class MaintenanceController(IMediator mediator) : GarageBaseController
{
    [HttpGet]
    [Route("~/api/vehicles/{vehicleId:guid}/maintenance-schedules")]
    [ProducesResponseType(typeof(SearchMaintenanceSchedulesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSchedules(
        [FromRoute] Guid vehicleId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchMaintenanceSchedulesQuery(vehicleId, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("~/api/vehicles/{vehicleId:guid}/maintenance-schedules")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateSchedule(
        [FromRoute] Guid vehicleId,
        [FromBody] CreateMaintenanceScheduleRequestDto request, 
        CancellationToken cancellationToken)
    {
        var command = new CreateMaintenanceScheduleCommand(vehicleId, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("~/api/vehicles/{vehicleId:guid}/maintenance-logs")]
    [ProducesResponseType(typeof(SearchMaintenanceLogsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLogs(
        [FromRoute] Guid vehicleId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchMaintenanceLogsQuery(vehicleId, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("~/api/vehicles/{vehicleId:guid}/maintenance-logs")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LogMaintenance(
        [FromRoute] Guid vehicleId,
        [FromBody] CreateLogMaintenanceRequestDto request, 
        CancellationToken cancellationToken)
    {
        var command = new LogMaintenanceCommand(vehicleId, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
