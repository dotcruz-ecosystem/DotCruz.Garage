using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.ProcessOdometerCheckin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Checkin;

[Route("api/[controller]")]
[ApiController]
[ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
public class CheckinController(IMediator mediator) : ControllerBase
{
    [HttpPost("odometer/{token}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OdometerCheckinResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProcessOdometerCheckin(
        [FromRoute] string token,
        [FromBody] ProcessOdometerCheckinRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new ProcessOdometerCheckinCommand(token, request.NewOdometer, request.RecordedAt);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
