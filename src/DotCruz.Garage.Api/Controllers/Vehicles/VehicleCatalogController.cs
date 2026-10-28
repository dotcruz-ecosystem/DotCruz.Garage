using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogBrands;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogEngines;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogModels;
using DotCruz.Shared.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Vehicles;

[Route("api/vehicles/catalog")]
public class VehicleCatalogController(IMediator mediator) : GarageBaseController
{
    [HttpGet("brands")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(IReadOnlyCollection<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogBrands([FromQuery] GetCatalogBrandsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("models")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(IReadOnlyCollection<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogModels([FromQuery] GetCatalogModelsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("engines")]
    [Authorize(Policy = SecurityPolicies.UserOnly)]
    [ProducesResponseType(typeof(IReadOnlyCollection<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogEngines([FromQuery] GetCatalogEnginesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
