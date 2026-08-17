using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Fines;
using DotCruz.Garage.Application.UseCases.Fines.Commands.CreateTrafficFine;
using DotCruz.Garage.Application.UseCases.Fines.Commands.PayTrafficFine;
using DotCruz.Garage.Application.UseCases.Fines.Queries.SearchTrafficFines;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Fines;

public class FinesController(IMediator mediator) : GarageBaseController
{
    [HttpGet]
    [Route("~/api/vehicles/{vehicleId:guid}/fines")]
    [ProducesResponseType(typeof(SearchTrafficFinesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByVehicle(
        [FromRoute] Guid vehicleId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchTrafficFinesQuery(vehicleId, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("~/api/vehicles/{vehicleId:guid}/fines")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Post(
        [FromRoute] Guid vehicleId,
        [FromBody] CreateTrafficFineRequestDto request, 
        CancellationToken cancellationToken)
    {
        var command = new CreateTrafficFineCommand(vehicleId, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPatch]
    [Route("{Id:guid}/pay")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Pay(
        [FromRoute] Guid Id, 
        [FromBody] PayFineRequestDto request, 
        CancellationToken cancellationToken)
    {
        var command = new PayTrafficFineCommand(Id, request);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
