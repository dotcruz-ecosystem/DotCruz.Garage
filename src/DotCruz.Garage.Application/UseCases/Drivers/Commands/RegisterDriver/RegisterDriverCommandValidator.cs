using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Drivers.Commands.RegisterDriver;

public class RegisterDriverCommandValidator : AbstractValidator<RegisterDriverCommand>
{
    public RegisterDriverCommandValidator()
    {
        RuleFor(x => x.RequestDto.Name)
            .NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY)
            .Length(2, 150).WithMessage(ResourceMessagesException.NAME_INVALID_LENGTH);

        RuleFor(x => x.RequestDto.Cpf)
            .NotEmpty().WithMessage(ResourceMessagesException.DRIVER_CPF_INVALID);

        RuleFor(x => x.RequestDto.Cnh)
            .NotEmpty().WithMessage(ResourceMessagesException.DRIVER_CNH_INVALID);

        RuleFor(x => x.RequestDto.CnhCategory)
            .NotEmpty().WithMessage(ResourceMessagesException.DRIVER_CNH_INVALID);

        RuleFor(x => x.RequestDto.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
