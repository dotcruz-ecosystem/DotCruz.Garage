using DotCruz.Garage.Domain.Entities.Base;

namespace DotCruz.Garage.Domain.Entities.ServiceOrders;

public class ServiceOrderAttachment : BaseEntity
{
    public Guid ServiceOrderId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string FileUrl { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long FileSize { get; private set; }

    public virtual ServiceOrder ServiceOrder { get; private init; } = default!;

    private ServiceOrderAttachment() { }

    public static ServiceOrderAttachment Create(
        Guid serviceOrderId,
        string fileName,
        string fileUrl,
        string contentType,
        long fileSize)
    {
        return new ServiceOrderAttachment
        {
            ServiceOrderId = serviceOrderId,
            FileName = fileName,
            FileUrl = fileUrl,
            ContentType = contentType,
            FileSize = fileSize
        };
    }
}
