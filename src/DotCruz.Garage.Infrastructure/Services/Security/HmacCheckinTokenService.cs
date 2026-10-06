using System.Security.Cryptography;
using System.Text;
using DotCruz.Garage.Application.Abstractions.Services.Security;
using DotCruz.Garage.Domain.ValueObjects;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace DotCruz.Garage.Infrastructure.Services.Security;

public class HmacCheckinTokenService : ICheckinTokenService
{
    private const string DefaultSecretKey = "DotCruzGarageSecureOdometerTokenHmacSecretKey2026";
    private readonly byte[] _secretKeyBytes;

    public HmacCheckinTokenService(IConfiguration configuration)
    {
        var secret = configuration["Settings:OdometerTokenSecret"]
                     ?? configuration["Jwt:Key"]
                     ?? DefaultSecretKey;

        _secretKeyBytes = Encoding.UTF8.GetBytes(secret);
    }

    public string GenerateToken(Guid vehicleId, Guid tenantId, TimeSpan? validity = null)
    {
        var duration = validity ?? TimeSpan.FromHours(48);
        var expiresAt = DateTimeOffset.UtcNow.Add(duration);
        var expiresUnix = expiresAt.ToUnixTimeSeconds();

        var payload = $"{vehicleId:N}:{tenantId:N}:{expiresUnix}";
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var base64UrlPayload = WebEncoders.Base64UrlEncode(payloadBytes);

        using var hmac = new HMACSHA256(_secretKeyBytes);
        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(base64UrlPayload));
        var base64UrlSignature = WebEncoders.Base64UrlEncode(signatureBytes);

        return $"{base64UrlPayload}.{base64UrlSignature}";
    }

    public bool TryValidateToken(string tokenString, out OdometerCheckinToken? token)
    {
        token = null;

        if (string.IsNullOrWhiteSpace(tokenString))
            return false;

        var parts = tokenString.Split('.');
        if (parts.Length != 2)
            return false;

        var base64UrlPayload = parts[0];
        var base64UrlSignature = parts[1];

        try
        {
            using var hmac = new HMACSHA256(_secretKeyBytes);
            var expectedSignatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(base64UrlPayload));
            var providedSignatureBytes = WebEncoders.Base64UrlDecode(base64UrlSignature);

            if (!CryptographicOperations.FixedTimeEquals(expectedSignatureBytes, providedSignatureBytes))
                return false;

            var payloadBytes = WebEncoders.Base64UrlDecode(base64UrlPayload);
            var payload = Encoding.UTF8.GetString(payloadBytes);

            var segments = payload.Split(':');
            if (segments.Length != 3)
                return false;

            if (!Guid.TryParseExact(segments[0], "N", out var vehicleId))
                return false;

            if (!Guid.TryParseExact(segments[1], "N", out var tenantId))
                return false;

            if (!long.TryParse(segments[2], out var expiresUnix))
                return false;

            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expiresUnix);
            token = new OdometerCheckinToken(vehicleId, tenantId, expiresAt);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GeneratePostSaleToken(Guid vehicleId, Guid saleId, Guid tenantId, TimeSpan? validity = null)
    {
        var duration = validity ?? TimeSpan.FromDays(180);
        var expiresAt = DateTimeOffset.UtcNow.Add(duration);
        var expiresUnix = expiresAt.ToUnixTimeSeconds();

        var payload = $"{vehicleId:N}:{saleId:N}:{tenantId:N}:{expiresUnix}";
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var base64UrlPayload = WebEncoders.Base64UrlEncode(payloadBytes);

        using var hmac = new HMACSHA256(_secretKeyBytes);
        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(base64UrlPayload));
        var base64UrlSignature = WebEncoders.Base64UrlEncode(signatureBytes);

        return $"{base64UrlPayload}.{base64UrlSignature}";
    }

    public bool TryValidatePostSaleToken(string tokenString, out PostSalePortalToken? token)
    {
        token = null;

        if (string.IsNullOrWhiteSpace(tokenString))
            return false;

        var parts = tokenString.Split('.');
        if (parts.Length != 2)
            return false;

        var base64UrlPayload = parts[0];
        var base64UrlSignature = parts[1];

        try
        {
            using var hmac = new HMACSHA256(_secretKeyBytes);
            var expectedSignatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(base64UrlPayload));
            var providedSignatureBytes = WebEncoders.Base64UrlDecode(base64UrlSignature);

            if (!CryptographicOperations.FixedTimeEquals(expectedSignatureBytes, providedSignatureBytes))
                return false;

            var payloadBytes = WebEncoders.Base64UrlDecode(base64UrlPayload);
            var payload = Encoding.UTF8.GetString(payloadBytes);

            var segments = payload.Split(':');
            if (segments.Length != 4)
                return false;

            if (!Guid.TryParseExact(segments[0], "N", out var vehicleId))
                return false;

            if (!Guid.TryParseExact(segments[1], "N", out var saleId))
                return false;

            if (!Guid.TryParseExact(segments[2], "N", out var tenantId))
                return false;

            if (!long.TryParse(segments[3], out var expiresUnix))
                return false;

            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expiresUnix);
            token = new PostSalePortalToken(vehicleId, saleId, tenantId, expiresAt);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
