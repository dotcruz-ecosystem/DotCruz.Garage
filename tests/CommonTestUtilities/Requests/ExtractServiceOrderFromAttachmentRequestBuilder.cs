using Bogus;
using DotCruz.Garage.Application.DTOs.ServiceOrders;

namespace CommonTestUtilities.Requests;

public class ExtractServiceOrderFromAttachmentRequestBuilder
{
    public static ExtractServiceOrderFromAttachmentRequestDto Build(
        string? fileUrl = null,
        string fileName = "ordem_de_servico.jpg",
        string contentType = "image/jpeg")
    {
        var faker = new Faker();

        return new ExtractServiceOrderFromAttachmentRequestDto(
            FileUrl: fileUrl ?? (faker.Internet.Url() + "/ordem.jpg"),
            FileName: fileName,
            ContentType: contentType);
    }
}
