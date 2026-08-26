using DotCruz.Garage.Application.DTOs.Catalog;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Catalog.Commands.ExtractManualMaintenance;

public record ExtractManualMaintenanceCommand(
    Stream FileStream,
    string FileName,
    string Brand,
    string Model,
    int Year,
    string? Engine,
    string? Version,
    Guid? VehicleId = null) : IRequest<VehicleBlueprintDto>;
