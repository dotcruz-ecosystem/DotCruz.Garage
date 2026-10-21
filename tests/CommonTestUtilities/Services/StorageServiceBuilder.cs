using DotCruz.Garage.Application.Abstractions.Services.Storage;
using DotCruz.Garage.Application.DTOs.Storage;
using Moq;

namespace CommonTestUtilities.Services;

public class StorageServiceBuilder
{
    private readonly Mock<IStorageService> _mock = new();

    public StorageServiceBuilder SetupGeneratePresignedUploadUrl(
        string uploadUrl = "https://r2.cloudflarestorage.com/upload-presigned", 
        string publicUrl = "https://pub.r2.dev/file.pdf")
    {
        _mock.Setup(s => s.GeneratePresignedUploadUrlAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StorageUploadUrlDto(uploadUrl, publicUrl));

        return this;
    }

    public IStorageService Build() => _mock.Object;
}
