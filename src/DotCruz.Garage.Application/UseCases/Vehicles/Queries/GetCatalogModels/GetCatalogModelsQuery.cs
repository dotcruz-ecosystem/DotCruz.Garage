using DotCruz.Garage.Domain.Enums;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogModels;

public sealed record GetCatalogModelsQuery(string Brand, VehicleType? Type = null) : IRequest<IReadOnlyCollection<string>>;
