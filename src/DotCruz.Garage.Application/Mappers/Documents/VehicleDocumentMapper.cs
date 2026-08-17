using DotCruz.Garage.Application.DTOs.Documents;
using DotCruz.Garage.Application.UseCases.Documents.Commands.CreateVehicleDocument;
using DotCruz.Garage.Domain.Entities.Documents;

namespace DotCruz.Garage.Application.Mappers.Documents;

public static class VehicleDocumentMapper
{
    public static VehicleDocument ToDomain(
        this CreateVehicleDocumentCommand command,
        Guid tenantId,
        Guid userId)
    {
        return VehicleDocument.Create(
            tenantId,
            userId,
            command.VehicleId,
            command.RequestDto.Type,
            command.RequestDto.ReferenceYear,
            command.RequestDto.DueDate,
            command.RequestDto.Cost,
            command.RequestDto.ExpirationDate,
            command.RequestDto.Status,
            command.RequestDto.FileUrl,
            command.RequestDto.WarningThresholdDays
        );
    }

    public static VehicleDocumentDto ToDto(this VehicleDocument doc)
    {
        return new VehicleDocumentDto(
            doc.Id,
            doc.VehicleId,
            doc.Type,
            doc.ReferenceYear,
            doc.DueDate,
            doc.Cost,
            doc.ExpirationDate,
            doc.Status.ToString(),
            doc.FileUrl,
            doc.WarningThresholdDays
        );
    }
}
