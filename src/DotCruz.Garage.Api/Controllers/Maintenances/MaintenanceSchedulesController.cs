using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.DTOs.ServiceOrders;
using DotCruz.Garage.Application.DTOs.Storage;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.CompleteMaintenanceSchedule;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.CreateMaintenanceSchedule;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.DeleteMaintenanceSchedule;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.UpdateMaintenanceSchedule;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetMaintenanceScheduleById;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetUploadUrl;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetVehicleMaintenanceOverview;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.SearchMaintenanceSchedules;
using DotCruz.Shared.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Maintenances;

[Route("api/vehicles/{vehicleId:guid}/maintenance-schedules")]
public class MaintenanceSchedulesController(IMediator mediator) : GarageBaseController
{
    [HttpGet]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(SearchMaintenanceSchedulesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSchedules(
        [FromRoute] Guid vehicleId,
        [FromQuery] PaginationQueryDto pagination,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchMaintenanceSchedulesQuery(vehicleId, pagination.PageNumber, pagination.PageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("overview")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(MaintenanceOverviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOverview(
        [FromRoute] Guid vehicleId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetVehicleMaintenanceOverviewQuery(vehicleId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{scheduleId:guid}")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(MaintenanceScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetScheduleById(
        [FromRoute] Guid vehicleId,
        [FromRoute] Guid scheduleId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMaintenanceScheduleByIdQuery(vehicleId, scheduleId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateSchedule(
        [FromRoute] Guid vehicleId,
        [FromBody] CreateMaintenanceScheduleRequestDto request, 
        CancellationToken cancellationToken)
    {
        var command = new CreateMaintenanceScheduleCommand(vehicleId, request);
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetScheduleById), new { vehicleId, scheduleId = result }, result);
    }

    [HttpPut("{scheduleId:guid}")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSchedule(
        [FromRoute] Guid vehicleId,
        [FromRoute] Guid scheduleId,
        [FromBody] UpdateMaintenanceScheduleRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMaintenanceScheduleCommand(vehicleId, scheduleId, request);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{scheduleId:guid}")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSchedule(
        [FromRoute] Guid vehicleId,
        [FromRoute] Guid scheduleId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteMaintenanceScheduleCommand(vehicleId, scheduleId);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("{scheduleId:guid}/complete")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(ServiceOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteSchedule(
        [FromRoute] Guid vehicleId,
        [FromRoute] Guid scheduleId,
        [FromBody] CompleteMaintenanceScheduleRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new CompleteMaintenanceScheduleCommand(vehicleId, scheduleId, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("~/api/vehicles/{vehicleId:guid}/maintenances/upload-url")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(StorageUploadUrlDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUploadUrl(
        [FromRoute] Guid vehicleId,
        [FromBody] GetMaintenanceUploadUrlRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetUploadUrlQuery(vehicleId, request.FileName, request.ContentType, request.FileSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
