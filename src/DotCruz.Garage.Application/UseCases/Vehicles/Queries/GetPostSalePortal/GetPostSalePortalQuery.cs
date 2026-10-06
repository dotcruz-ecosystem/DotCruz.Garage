using DotCruz.Garage.Application.DTOs.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetPostSalePortal;

public sealed record GetPostSalePortalQuery(string Token) : IRequest<PostSalePortalDto>;
