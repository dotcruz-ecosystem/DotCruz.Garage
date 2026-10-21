using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetUploadUrl;

public class GetUploadUrlQueryValidator : AbstractValidator<GetUploadUrlQuery>
{
    private static readonly string[] AllowedContentTypes =
    [
        "application/pdf",
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp"
    ];

    private const long MaxFileSizeBytes = 15 * 1024 * 1024; // 15 MB

    public GetUploadUrlQueryValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("O ID do veículo é obrigatório.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("O nome do arquivo é obrigatório.")
            .MaximumLength(255).WithMessage("O nome do arquivo não pode exceder 255 caracteres.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("O tipo de conteúdo (MIME type) é obrigatório.")
            .Must(ct => AllowedContentTypes.Contains(ct.Trim().ToLowerInvariant()))
            .WithMessage("Formato de arquivo não suportado. Utilize PDF, JPEG, PNG ou WebP.");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("O arquivo deve ter tamanho maior que 0 bytes.")
            .LessThanOrEqualTo(MaxFileSizeBytes).WithMessage("O tamanho máximo do arquivo é de 15 MB.");
    }
}
