using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Accessories;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.UseCases.Accessories.Commands.CreateVehicleAccessory;
using DotCruz.Garage.Application.UseCases.Accessories.Queries.SearchVehicleAccessories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Accessories;

public class AccessoriesController(IMediator mediator) : GarageBaseController
{
    [HttpGet]
    [Route("~/api/vehicles/{vehicleId:guid}/accessories")]
    [ProducesResponseType(typeof(SearchVehicleAccessoriesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByVehicle(
        [FromRoute] Guid vehicleId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchVehicleAccessoriesQuery(vehicleId, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("~/api/vehicles/{vehicleId:guid}/accessories")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Post(
        [FromRoute] Guid vehicleId,
        [FromBody] CreateVehicleAccessoryRequestDto request, 
        CancellationToken cancellationToken)
    {
        var command = new CreateVehicleAccessoryCommand(vehicleId, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
