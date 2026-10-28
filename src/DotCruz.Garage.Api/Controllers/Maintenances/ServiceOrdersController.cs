using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.ServiceOrders;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.AddServiceOrderAttachment;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.CreateServiceOrder;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.RemoveServiceOrderAttachment;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.ExtractServiceOrderFromAttachment;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.GetServiceOrderById;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.GetServiceOrdersByVehicle;
using DotCruz.Shared.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Maintenances;

[Route("api/vehicles/{vehicleId:guid}/service-orders")]
public class ServiceOrdersController(IMediator mediator) : GarageBaseController
{
    [HttpGet]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
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

    [HttpGet("{serviceOrderId:guid}")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
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
    [Authorize(Policy = SecurityPolicies.UserOnly)]
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

    [HttpPost("extract-from-attachment")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(ExtractedServiceOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExtractFromAttachment(
        [FromRoute] Guid vehicleId,
        [FromBody] ExtractServiceOrderFromAttachmentRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = new ExtractServiceOrderFromAttachmentQuery(vehicleId, request);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{serviceOrderId:guid}/attachments")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
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

    [HttpDelete("{serviceOrderId:guid}/attachments/{attachmentId:guid}")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
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
}
