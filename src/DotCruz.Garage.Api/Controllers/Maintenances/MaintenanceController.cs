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
using DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.AddServiceOrderAttachment;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.CreateServiceOrder;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.RemoveServiceOrderAttachment;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.GetServiceOrderById;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.GetServiceOrdersByVehicle;
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
        [FromQuery] PaginationQueryDto pagination,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchMaintenanceSchedulesQuery(vehicleId, pagination.PageNumber, pagination.PageSize);
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

    [HttpGet]
    [Route("~/api/vehicles/{vehicleId:guid}/service-orders")]
    [ProducesResponseType(typeof(SearchServiceOrdersResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetServiceOrders(
        [FromRoute] Guid vehicleId,
        [FromQuery] PaginationQueryDto pagination,
        CancellationToken cancellationToken = default)
    {
        var query = new GetServiceOrdersByVehicleQuery(vehicleId, pagination.PageNumber, pagination.PageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("~/api/vehicles/{vehicleId:guid}/service-orders/{serviceOrderId:guid}")]
    [ProducesResponseType(typeof(ServiceOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetServiceOrderById(
        [FromRoute] Guid vehicleId,
        [FromRoute] Guid serviceOrderId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetServiceOrderByIdQuery(serviceOrderId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("~/api/vehicles/{vehicleId:guid}/service-orders")]
    [ProducesResponseType(typeof(ServiceOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateServiceOrder(
        [FromRoute] Guid vehicleId,
        [FromBody] CreateServiceOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateServiceOrderCommand(vehicleId, request);
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetServiceOrderById), new { vehicleId, serviceOrderId = result.Id }, result);
    }

    [HttpPost]
    [Route("~/api/vehicles/{vehicleId:guid}/service-orders/{serviceOrderId:guid}/attachments")]
    [ProducesResponseType(typeof(ServiceOrderAttachmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddServiceOrderAttachment(
        [FromRoute] Guid vehicleId,
        [FromRoute] Guid serviceOrderId,
        [FromBody] CreateServiceOrderAttachmentRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new AddServiceOrderAttachmentCommand(
            serviceOrderId,
            request.FileName,
            request.FileUrl,
            request.ContentType,
            request.FileSize);
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete]
    [Route("~/api/vehicles/{vehicleId:guid}/service-orders/{serviceOrderId:guid}/attachments/{attachmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveServiceOrderAttachment(
        [FromRoute] Guid vehicleId,
        [FromRoute] Guid serviceOrderId,
        [FromRoute] Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveServiceOrderAttachmentCommand(serviceOrderId, attachmentId);
        await mediator.Send(command, cancellationToken);
        return NoContent();
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
}
