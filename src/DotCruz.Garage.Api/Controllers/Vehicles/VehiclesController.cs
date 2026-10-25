using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Blueprints;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Application.UseCases.Blueprints.Commands.ApplyBlueprintToVehicle;
using DotCruz.Garage.Application.UseCases.Blueprints.Commands.RequestVehicleBlueprint;
using DotCruz.Garage.Application.UseCases.Blueprints.Queries.GetVehicleBlueprintByVehicleId;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.UpdateOdometer;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogBrands;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogEngines;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogModels;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetVehicleById;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.SearchVehicles;
using DotCruz.Garage.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Vehicles;

public class VehiclesController(IMediator mediator) : GarageBaseController
{
    [HttpGet("catalog/brands")]
    [ProducesResponseType(typeof(IReadOnlyCollection<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogBrands([FromQuery] GetCatalogBrandsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("catalog/models")]
    [ProducesResponseType(typeof(IReadOnlyCollection<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogModels([FromQuery] GetCatalogModelsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("catalog/engines")]
    [ProducesResponseType(typeof(IReadOnlyCollection<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogEngines([FromQuery] GetCatalogEnginesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [ProducesResponseType(typeof(SearchVehiclesResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] SearchVehiclesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("{Id:guid}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        var query = new GetVehicleByIdQuery(Id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateVehicleCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { Id = result }, result);
    }

    [HttpPatch]
    [Route("{Id:guid}/odometer")]
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

    [HttpGet]
    [Route("{Id:guid}/blueprint")]
    [ProducesResponseType(typeof(VehicleBlueprintDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBlueprint([FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        var query = new GetVehicleBlueprintByVehicleIdQuery(Id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("{Id:guid}/apply-blueprint")]
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
