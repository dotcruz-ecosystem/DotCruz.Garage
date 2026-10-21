using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.AddMaintenanceAttachment;

public class AddMaintenanceAttachmentCommandHandler(
    IMaintenanceLogWriteRepository maintenanceLogWriteRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddMaintenanceAttachmentCommand, MaintenanceAttachmentDto>
{
    public async Task<MaintenanceAttachmentDto> Handle(AddMaintenanceAttachmentCommand request, CancellationToken cancellationToken)
    {
        var log = await maintenanceLogWriteRepository.GetByIdWithAttachmentsToUpdateAsync(request.MaintenanceLogId, cancellationToken);
        if (log == null || log.VehicleId != request.VehicleId)
        {
            throw new NotFoundException("Registro de manutenção não encontrado para o veículo especificado.");
        }

        var attachment = log.AddAttachment(
            request.RequestDto.FileName,
            request.RequestDto.FileUrl,
            request.RequestDto.ContentType,
            request.RequestDto.FileSize);

        await unitOfWork.CommitAsync(cancellationToken);

        return new MaintenanceAttachmentDto(
            attachment.Id,
            attachment.MaintenanceLogId,
            attachment.FileName,
            attachment.FileUrl,
            attachment.ContentType,
            attachment.FileSize,
            attachment.CreatedAt);
    }
}
