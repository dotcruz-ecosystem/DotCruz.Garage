using DotCruz.Garage.Domain.Enums;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogBrands;

public sealed record GetCatalogBrandsQuery(VehicleType? Type = null) : IRequest<IReadOnlyCollection<string>>;
