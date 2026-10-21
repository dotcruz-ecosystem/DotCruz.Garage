using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.AddMaintenanceAttachment;

public class AddMaintenanceAttachmentCommandValidator : AbstractValidator<AddMaintenanceAttachmentCommand>
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

    public AddMaintenanceAttachmentCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("O ID do veículo é obrigatório.");

        RuleFor(x => x.MaintenanceLogId)
            .NotEmpty().WithMessage("O ID do registro de manutenção é obrigatório.");

        RuleFor(x => x.RequestDto.FileName)
            .NotEmpty().WithMessage("O nome do arquivo é obrigatório.")
            .MaximumLength(255).WithMessage("O nome do arquivo não pode exceder 255 caracteres.");

        RuleFor(x => x.RequestDto.FileUrl)
            .NotEmpty().WithMessage("A URL do arquivo é obrigatória.")
            .MaximumLength(1000).WithMessage("A URL do arquivo não pode exceder 1000 caracteres.");

        RuleFor(x => x.RequestDto.ContentType)
            .NotEmpty().WithMessage("O tipo de conteúdo é obrigatório.")
            .Must(ct => AllowedContentTypes.Contains(ct.Trim().ToLowerInvariant()))
            .WithMessage("Formato de arquivo não suportado. Utilize PDF, JPEG, PNG ou WebP.");

        RuleFor(x => x.RequestDto.FileSize)
            .GreaterThan(0).WithMessage("O tamanho do arquivo deve ser maior que 0.")
            .LessThanOrEqualTo(MaxFileSizeBytes).WithMessage("O tamanho máximo do arquivo é de 15 MB.");
    }
}
