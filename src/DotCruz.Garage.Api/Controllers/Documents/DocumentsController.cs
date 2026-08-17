using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Documents;
using DotCruz.Garage.Application.UseCases.Documents.Commands.CreateVehicleDocument;
using DotCruz.Garage.Application.UseCases.Documents.Commands.PayVehicleDocument;
using DotCruz.Garage.Application.UseCases.Documents.Queries.SearchVehicleDocuments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Documents;

public class DocumentsController(IMediator mediator) : GarageBaseController
{
    [HttpGet]
    [Route("~/api/vehicles/{vehicleId:guid}/documents")]
    [ProducesResponseType(typeof(SearchVehicleDocumentsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByVehicle(
        [FromRoute] Guid vehicleId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchVehicleDocumentsQuery(vehicleId, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("~/api/vehicles/{vehicleId:guid}/documents")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Post(
        [FromRoute] Guid vehicleId,
        [FromBody] CreateVehicleDocumentRequestDto request, 
        CancellationToken cancellationToken)
    {
        var command = new CreateVehicleDocumentCommand(vehicleId, request);
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
        [FromBody] PayDocumentRequestDto request, 
        CancellationToken cancellationToken)
    {
        var command = new PayVehicleDocumentCommand(Id, request);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
