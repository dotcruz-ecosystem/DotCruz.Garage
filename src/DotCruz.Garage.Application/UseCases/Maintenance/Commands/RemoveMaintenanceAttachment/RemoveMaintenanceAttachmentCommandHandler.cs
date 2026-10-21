using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.RemoveMaintenanceAttachment;

public class RemoveMaintenanceAttachmentCommandHandler(
    IMaintenanceLogWriteRepository maintenanceLogWriteRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveMaintenanceAttachmentCommand>
{
    public async Task Handle(RemoveMaintenanceAttachmentCommand request, CancellationToken cancellationToken)
    {
        var log = await maintenanceLogWriteRepository.GetByIdWithAttachmentsToUpdateAsync(request.MaintenanceLogId, cancellationToken);
        if (log == null || log.VehicleId != request.VehicleId)
        {
            throw new NotFoundException("Registro de manutenção não encontrado para o veículo especificado.");
        }

        var attachmentExists = log.Attachments.Any(a => a.Id == request.AttachmentId);
        if (!attachmentExists)
        {
            throw new NotFoundException("Anexo não encontrado para este registro de manutenção.");
        }

        log.RemoveAttachment(request.AttachmentId);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
