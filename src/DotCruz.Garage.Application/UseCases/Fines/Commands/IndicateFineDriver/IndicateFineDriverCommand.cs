using DotCruz.Garage.Application.DTOs.Fines;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Fines.Commands.IndicateFineDriver;

public record IndicateFineDriverCommand(Guid FineId, IndicateFineDriverRequestDto RequestDto) : IRequest;
