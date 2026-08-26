using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Catalog.Commands.ExtractManualMaintenance;

public class ExtractManualMaintenanceCommandValidator : AbstractValidator<ExtractManualMaintenanceCommand>
{
    public ExtractManualMaintenanceCommandValidator()
    {
        RuleFor(x => x.Brand)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.BRAND_EMPTY);

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.MODEL_EMPTY);

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 2)
            .WithMessage(ResourceMessagesException.YEAR_INVALID);

        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage(ResourceMessagesException.MANUAL_FILE_REQUIRED);

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.MANUAL_FILE_REQUIRED)
            .Must(fileName => fileName != null && fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage(ResourceMessagesException.MANUAL_FILE_INVALID_FORMAT);
    }
}
