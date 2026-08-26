using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Drivers;
using DotCruz.Garage.Application.UseCases.Drivers.Commands.AssignDriverToVehicle;
using DotCruz.Garage.Application.UseCases.Drivers.Commands.DeleteDriver;
using DotCruz.Garage.Application.UseCases.Drivers.Commands.RegisterDriver;
using DotCruz.Garage.Application.UseCases.Drivers.Commands.UpdateDriver;
using DotCruz.Garage.Application.UseCases.Drivers.Queries.GetDriverById;
using DotCruz.Garage.Application.UseCases.Drivers.Queries.GetDriverFinesSummary;
using DotCruz.Garage.Application.UseCases.Drivers.Queries.GetVehicleDriverHistory;
using DotCruz.Garage.Application.UseCases.Drivers.Queries.SearchDrivers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Drivers;

public class DriversController(IMediator mediator) : GarageBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(SearchDriversResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? activeOnly = null,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchDriversQuery(pageNumber, pageSize, searchTerm, activeOnly);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("{id:guid}")]
    [ProducesResponseType(typeof(DriverDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetDriverByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDriverRequestDto request, CancellationToken cancellationToken)
    {
        var command = new RegisterDriverCommand(request);
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateDriverRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateDriverCommand(id, request);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteDriverCommand(id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet]
    [Route("{id:guid}/fines-summary")]
    [ProducesResponseType(typeof(DriverFinesSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFinesSummary([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetDriverFinesSummaryQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("{id:guid}/assign-vehicle")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignVehicle(
        [FromRoute] Guid id,
        [FromBody] AssignDriverRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new AssignDriverToVehicleCommand(id, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("~/api/vehicles/{vehicleId:guid}/driver-history")]
    [ProducesResponseType(typeof(IReadOnlyCollection<VehicleDriverAssignmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVehicleDriverHistory([FromRoute] Guid vehicleId, CancellationToken cancellationToken)
    {
        var query = new GetVehicleDriverHistoryQuery(vehicleId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
