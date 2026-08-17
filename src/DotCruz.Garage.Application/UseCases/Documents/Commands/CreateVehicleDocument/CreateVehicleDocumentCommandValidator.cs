using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Documents.Commands.CreateVehicleDocument;

public class CreateVehicleDocumentCommandValidator : AbstractValidator<CreateVehicleDocumentCommand>
{
    public CreateVehicleDocumentCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.Type)
            .IsInEnum();

        RuleFor(x => x.RequestDto.ReferenceYear)
            .InclusiveBetween(1900, 2100);

        RuleFor(x => x.RequestDto.Cost)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.RequestDto.Status)
            .IsInEnum();

        RuleFor(x => x.RequestDto.WarningThresholdDays)
            .GreaterThan(0).When(x => x.RequestDto.WarningThresholdDays.HasValue);
    }
}
