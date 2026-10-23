using DotCruz.Garage.Domain.Enums;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogEngines;

public sealed record GetCatalogEnginesQuery(VehicleType? Type = null) : IRequest<IReadOnlyCollection<string>>;
