using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.UpdateOdometer;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetVehicleById;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.SearchVehicles;
using DotCruz.Shared.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Vehicles;

public class VehiclesController(IMediator mediator) : GarageBaseController
{
    [HttpGet]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(SearchVehiclesResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] SearchVehiclesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("{Id:guid}")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        var query = new GetVehicleByIdQuery(Id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateVehicleCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { Id = result }, result);
    }

    [HttpPatch]
    [Route("{Id:guid}/odometer")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOdometer(
        [FromRoute] Guid Id, 
        [FromBody] UpdateOdometerRequestDto request, 
        CancellationToken cancellationToken)
    {
        var command = new UpdateOdometerCommand(Id, request);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
