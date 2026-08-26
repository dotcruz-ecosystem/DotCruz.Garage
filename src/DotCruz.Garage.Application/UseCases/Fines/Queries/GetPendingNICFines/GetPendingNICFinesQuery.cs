using DotCruz.Garage.Application.DTOs.Fines;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Fines.Queries.GetPendingNICFines;

public record GetPendingNICFinesQuery() : IRequest<IReadOnlyCollection<NICFineAlertDto>>;
