using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Catalog;
using DotCruz.Garage.Application.UseCases.Catalog.Commands.ApplyBlueprintToVehicle;
using DotCruz.Garage.Application.UseCases.Catalog.Commands.ExtractManualMaintenance;
using DotCruz.Garage.Application.UseCases.Catalog.Queries.GetVehicleBlueprintById;
using DotCruz.Garage.Application.UseCases.Catalog.Queries.SearchVehicleBlueprints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Catalog;

public class CatalogController(IMediator mediator) : GarageBaseController
{
    [HttpGet("blueprints")]
    [ProducesResponseType(typeof(SearchVehicleBlueprintsResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchBlueprints(
        [FromQuery] SearchVehicleBlueprintsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("blueprints/{id:guid}")]
    [ProducesResponseType(typeof(VehicleBlueprintDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBlueprintById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetVehicleBlueprintByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("manuals/extract")]
    [HttpPost("~/api/vehicles/manuals/extract")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(VehicleBlueprintDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExtractManual(
        IFormFile file,
        [FromForm] string brand,
        [FromForm] string model,
        [FromForm] int year,
        [FromForm] string? engine,
        [FromForm] string? version,
        [FromForm] Guid? vehicleId,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new ErrorResponseDto([ "Manual file is required." ]));
        }

        using var stream = file.OpenReadStream();
        var command = new ExtractManualMaintenanceCommand(
            stream,
            file.FileName,
            brand,
            model,
            year,
            engine,
            version,
            vehicleId);

        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetBlueprintById), new { id = result.Id }, result);
    }

    [HttpPost("vehicles/{vehicleId:guid}/apply-blueprint/{blueprintId:guid}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApplyBlueprint(
        [FromRoute] Guid vehicleId,
        [FromRoute] Guid blueprintId,
        CancellationToken cancellationToken)
    {
        var command = new ApplyBlueprintToVehicleCommand(vehicleId, blueprintId);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(new { schedulesCreated = result });
    }
}
