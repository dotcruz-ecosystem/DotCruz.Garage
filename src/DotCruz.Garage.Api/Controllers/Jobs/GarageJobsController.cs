using DotCruz.Garage.Application.UseCases.Alerts.Commands.CheckAlerts;
using DotCruz.Shared.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Jobs;

[ApiController]
[Authorize(Policy = SecurityPolicies.ServiceOnly)]
public class GarageJobsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Route("check-alerts")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckAlerts(CancellationToken cancellationToken)
    {
        await mediator.Send(new CheckAlertsCommand(), cancellationToken);
        return NoContent();
    }
}
