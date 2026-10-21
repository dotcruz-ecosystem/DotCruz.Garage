using CommonTestUtilities.Entities;
using DotCruz.Garage.Domain.Entities.Maintenance;

namespace UseCases.Test.Maintenance;

public class MaintenanceAttachmentDomainTest
{
    [Fact]
    public void Successfully_Creates_Attachment_And_Adds_To_MaintenanceLog()
    {
        var log = MaintenanceLogBuilder.Build();
        Assert.Empty(log.Attachments);

        var attachment = log.AddAttachment(
            "nota_fiscal_oleo.pdf",
            "https://pub.r2.dev/workspaces/1/vehicles/2/receipts/nota_fiscal_oleo.pdf",
            "application/pdf",
            1024 * 500);

        Assert.NotNull(attachment);
        Assert.Equal(log.Id, attachment.MaintenanceLogId);
        Assert.Equal("nota_fiscal_oleo.pdf", attachment.FileName);
        Assert.Equal("application/pdf", attachment.ContentType);
        Assert.Equal(1024 * 500, attachment.FileSize);
        Assert.Single(log.Attachments);
    }

    [Fact]
    public void Successfully_Removes_Attachment_From_MaintenanceLog()
    {
        var log = MaintenanceLogBuilder.Build();
        var attachment = log.AddAttachment(
            "foto_comprovante.jpg",
            "https://pub.r2.dev/comprovante.jpg",
            "image/jpeg",
            200_000);

        Assert.Single(log.Attachments);

        log.RemoveAttachment(attachment.Id);
        Assert.Empty(log.Attachments);
    }

    [Theory]
    [InlineData("", "https://url.com", "application/pdf")]
    [InlineData("file.pdf", "", "application/pdf")]
    [InlineData("file.pdf", "https://url.com", "")]
    public void Throws_ArgumentException_On_Invalid_Attachment_Creation(string fileName, string fileUrl, string contentType)
    {
        Assert.Throws<ArgumentException>(() =>
            MaintenanceAttachment.Create(Guid.NewGuid(), fileName, fileUrl, contentType, 100));
    }
}
