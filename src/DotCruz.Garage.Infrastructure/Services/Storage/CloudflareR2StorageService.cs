using Amazon.S3;
using Amazon.S3.Model;
using DotCruz.Garage.Application.Abstractions.Services.Storage;
using DotCruz.Garage.Application.DTOs.Storage;
using Microsoft.Extensions.Options;

namespace DotCruz.Garage.Infrastructure.Services.Storage;

public class CloudflareR2StorageService : IStorageService
{
    private readonly CloudflareR2Settings _settings;

    public CloudflareR2StorageService(IOptions<CloudflareR2Settings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<StorageUploadUrlDto> GeneratePresignedUploadUrlAsync(
        Guid workspaceId,
        Guid vehicleId,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        var s3Config = new AmazonS3Config
        {
            ServiceURL = $"https://{_settings.AccountId}.r2.cloudflarestorage.com",
            ForcePathStyle = true
        };

        using var s3Client = new AmazonS3Client(_settings.AccessKey, _settings.SecretKey, s3Config);

        var cleanFileName = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var uniqueFileName = $"{cleanFileName}-{timestamp}{extension}";

        var objectKey = $"workspaces/{workspaceId}/vehicles/{vehicleId}/receipts/{uniqueFileName}";

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _settings.BucketName,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.AddMinutes(15)
        };

        var uploadUrl = await s3Client.GetPreSignedURLAsync(request);

        var publicBaseUrl = _settings.PublicBaseUrl.TrimEnd('/');
        var publicUrl = $"{publicBaseUrl}/{objectKey}";

        return new StorageUploadUrlDto(uploadUrl, publicUrl);
    }
}
