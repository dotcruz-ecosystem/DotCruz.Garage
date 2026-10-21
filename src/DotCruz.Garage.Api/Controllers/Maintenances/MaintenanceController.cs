using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.CreateMaintenanceSchedule;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.CompleteMaintenanceSchedule;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.DeleteMaintenanceSchedule;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.LogMaintenance;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.UpdateMaintenanceSchedule;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetMaintenanceScheduleById;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetVehicleMaintenanceOverview;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.SearchMaintenanceLogs;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.SearchMaintenanceSchedules;
using DotCruz.Garage.Application.DTOs.Storage;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.AddMaintenanceAttachment;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.RemoveMaintenanceAttachment;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetUploadUrl;
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

    [HttpGet]
    [Route("~/api/vehicles/{vehicleId:guid}/maintenance-schedules/overview")]
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

    [HttpGet]
    [Route("~/api/vehicles/{vehicleId:guid}/maintenance-schedules/{scheduleId:guid}")]
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
    [Route("~/api/vehicles/{vehicleId:guid}/maintenance-schedules")]
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

    [HttpPut]
    [Route("~/api/vehicles/{vehicleId:guid}/maintenance-schedules/{scheduleId:guid}")]
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

    [HttpDelete]
    [Route("~/api/vehicles/{vehicleId:guid}/maintenance-schedules/{scheduleId:guid}")]
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

    [HttpPost]
    [Route("~/api/vehicles/{vehicleId:guid}/maintenance-schedules/{scheduleId:guid}/complete")]
    [ProducesResponseType(typeof(MaintenanceLogDto), StatusCodes.Status200OK)]
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

    [HttpPost]
    [Route("~/api/vehicles/{vehicleId:guid}/maintenances/upload-url")]
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

    [HttpPost]
    [Route("~/api/vehicles/{vehicleId:guid}/maintenance-logs/{logId:guid}/attachments")]
    [ProducesResponseType(typeof(MaintenanceAttachmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddAttachment(
        [FromRoute] Guid vehicleId,
        [FromRoute] Guid logId,
        [FromBody] CreateMaintenanceAttachmentDto request,
        CancellationToken cancellationToken)
    {
        var command = new AddMaintenanceAttachmentCommand(vehicleId, logId, request);
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete]
    [Route("~/api/vehicles/{vehicleId:guid}/maintenance-logs/{logId:guid}/attachments/{attachmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveAttachment(
        [FromRoute] Guid vehicleId,
        [FromRoute] Guid logId,
        [FromRoute] Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveMaintenanceAttachmentCommand(vehicleId, logId, attachmentId);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
