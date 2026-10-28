using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.ExtractServiceOrderFromAttachment;

public class ExtractServiceOrderFromAttachmentQueryValidator : AbstractValidator<ExtractServiceOrderFromAttachmentQuery>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp",
        "application/pdf"
    ];

    public ExtractServiceOrderFromAttachmentQueryValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("O veículo é obrigatório.");

        RuleFor(x => x.RequestDto.FileUrl)
            .NotEmpty().WithMessage("A URL do arquivo é obrigatória.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("A URL do arquivo é inválida.");

        RuleFor(x => x.RequestDto.FileName)
            .NotEmpty().WithMessage("O nome do arquivo é obrigatório.");

        RuleFor(x => x.RequestDto.ContentType)
            .NotEmpty().WithMessage("O tipo de conteúdo é obrigatório.")
            .Must(ct => AllowedContentTypes.Contains(ct.ToLowerInvariant()))
            .WithMessage("Formato de arquivo não suportado. Utilize JPG, PNG, WEBP ou PDF.");
    }
}
