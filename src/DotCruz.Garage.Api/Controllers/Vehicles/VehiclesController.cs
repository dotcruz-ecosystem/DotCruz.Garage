using DotCruz.Garage.Api.Controllers.Base;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.GenerateOdometerCheckinToken;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.RegisterVehicleSale;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.ClaimPostSaleVehicle;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.UpdateOdometer;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetPostSaleVehicles;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetVehicleById;
using DotCruz.Garage.Application.UseCases.Vehicles.Queries.SearchVehicles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Vehicles;

public class VehiclesController(IMediator mediator) : GarageBaseController
{
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

    [HttpPost]
    [Route("{Id:guid}/checkin-token")]
    [ProducesResponseType(typeof(GenerateOdometerCheckinTokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateCheckinToken(
        [FromRoute] Guid Id,
        CancellationToken cancellationToken)
    {
        var command = new GenerateOdometerCheckinTokenCommand(Id);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("{Id:guid}/sale")]
    [ProducesResponseType(typeof(RegisterVehicleSaleResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegisterSale(
        [FromRoute] Guid Id,
        [FromBody] RegisterVehicleSaleRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterVehicleSaleCommand(
            Id,
            request.CustomerName,
            request.CustomerEmail,
            request.CustomerPhone,
            request.CustomerDocument,
            request.SoldAt,
            request.SoldPrice,
            request.SoldOdometer,
            request.WarrantyExpiresAt,
            request.WarrantyKm,
            request.Notes
        );

        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet]
    [Route("post-sale")]
    [ProducesResponseType(typeof(IReadOnlyCollection<VehicleSaleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostSaleVehicles(CancellationToken cancellationToken)
    {
        var query = new DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetPostSaleVehicles.GetPostSaleVehiclesQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("post-sale/claim")]
    [ProducesResponseType(typeof(ClaimPostSaleVehicleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClaimPostSaleVehicle(
        [FromBody] ClaimVehicleRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new DotCruz.Garage.Application.UseCases.Vehicles.Commands.ClaimPostSaleVehicle.ClaimPostSaleVehicleCommand(request.Token);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
