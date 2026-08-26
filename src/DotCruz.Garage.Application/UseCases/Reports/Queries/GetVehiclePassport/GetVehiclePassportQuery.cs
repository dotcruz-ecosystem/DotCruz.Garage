using DotCruz.Garage.Application.DTOs.Reports;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Reports.Queries.GetVehiclePassport;

public record GetVehiclePassportQuery(Guid VehicleId) : IRequest<VehiclePassportDto>;
