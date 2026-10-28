using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Blueprints;
using DotCruz.Garage.Application.UseCases.Blueprints.Commands.ApplyBlueprintToVehicle;
using DotCruz.Garage.Application.UseCases.Blueprints.Queries.GetVehicleBlueprintByVehicleId;
using DotCruz.Shared.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Vehicles;

[Route("api/vehicles")]
public class VehicleBlueprintsController(IMediator mediator) : GarageBaseController
{
    [HttpGet("{Id:guid}/blueprint")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(VehicleBlueprintDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBlueprint([FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        var query = new GetVehicleBlueprintByVehicleIdQuery(Id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{Id:guid}/apply-blueprint")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(IReadOnlyCollection<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApplyBlueprint(
        [FromRoute] Guid Id,
        [FromBody] ApplyBlueprintRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new ApplyBlueprintToVehicleCommand(Id, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
