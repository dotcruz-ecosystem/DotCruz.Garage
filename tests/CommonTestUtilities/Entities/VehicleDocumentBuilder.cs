using Bogus;
using DotCruz.Garage.Domain.Entities.Documents;
using DotCruz.Garage.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class VehicleDocumentBuilder
{
    public static VehicleDocument Build(
        Guid? workspaceId = null,
        Guid? userId = null,
        Guid? vehicleId = null,
        DocumentType type = DocumentType.IPVA,
        int? referenceYear = null,
        decimal cost = 1200.00m,
        DateTimeOffset? dueDate = null,
        DocumentStatus status = DocumentStatus.Pending)
    {
        return VehicleDocument.Create(
            workspaceId: workspaceId ?? Guid.NewGuid(),
            userId: userId ?? Guid.NewGuid(),
            vehicleId: vehicleId ?? Guid.NewGuid(),
            type: type,
            referenceYear: referenceYear ?? DateTimeOffset.UtcNow.Year,
            dueDate: dueDate ?? DateTimeOffset.UtcNow.AddMonths(1),
            cost: cost,
            expirationDate: DateTimeOffset.UtcNow.AddYears(1),
            status: status,
            fileUrl: null,
            warningThresholdDays: 30
        );
    }
}
