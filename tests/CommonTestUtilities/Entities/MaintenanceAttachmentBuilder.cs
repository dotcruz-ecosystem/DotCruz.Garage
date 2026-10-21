using Bogus;
using DotCruz.Garage.Domain.Entities.Maintenance;

namespace CommonTestUtilities.Entities;

public class MaintenanceAttachmentBuilder
{
    public static MaintenanceAttachment Build(
        Guid? maintenanceLogId = null,
        string? fileName = null,
        string? fileUrl = null,
        string? contentType = null,
        long? fileSize = null)
    {
        var faker = new Faker();

        return MaintenanceAttachment.Create(
            maintenanceLogId: maintenanceLogId ?? Guid.NewGuid(),
            fileName: fileName ?? faker.System.FileName("pdf"),
            fileUrl: fileUrl ?? faker.Internet.Url(),
            contentType: contentType ?? "application/pdf",
            fileSize: fileSize ?? faker.Random.Long(50_000, 5_000_000)
        );
    }
}
