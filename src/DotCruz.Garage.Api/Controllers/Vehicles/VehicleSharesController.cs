using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.RevokeVehicleShare;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.ShareVehicle;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.TransferVehicleOwnership;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetVehicleShares;
using DotCruz.Shared.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Vehicles;

[Route("api/vehicles")]
public class VehicleSharesController(IMediator mediator) : GarageBaseController
{
    [HttpGet("{Id:guid}/shares")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(IReadOnlyCollection<VehicleShareDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShares([FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        var query = new GetVehicleSharesQuery(Id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{Id:guid}/shares")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(VehicleShareDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ShareVehicle(
        [FromRoute] Guid Id,
        [FromBody] ShareVehicleRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new ShareVehicleCommand(Id, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{Id:guid}/shares/{targetWorkspaceId:guid}")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeShare(
        [FromRoute] Guid Id,
        [FromRoute] Guid targetWorkspaceId,
        CancellationToken cancellationToken)
    {
        var command = new RevokeVehicleShareCommand(Id, targetWorkspaceId);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("{Id:guid}/transfer-ownership")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransferOwnership(
        [FromRoute] Guid Id,
        [FromBody] TransferOwnershipRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new TransferVehicleOwnershipCommand(Id, request);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
