using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.SubmitPostSaleOdometer;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetPostSalePortal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.PostSale;

[Route("api/public/post-sale")]
[ApiController]
[AllowAnonymous]
[ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
public class PostSalePortalController(IMediator mediator) : ControllerBase
{
    [HttpGet("portal/{token}")]
    [ProducesResponseType(typeof(PostSalePortalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPortal([FromRoute] string token, CancellationToken cancellationToken)
    {
        var query = new GetPostSalePortalQuery(token);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("odometer/{token}")]
    [ProducesResponseType(typeof(OdometerCheckinResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitOdometer(
        [FromRoute] string token,
        [FromBody] SubmitPostSaleOdometerRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new SubmitPostSaleOdometerCommand(token, request.NewOdometer, request.RecordedAt);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
