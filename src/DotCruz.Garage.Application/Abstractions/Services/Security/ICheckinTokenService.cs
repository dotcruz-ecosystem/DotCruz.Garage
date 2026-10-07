using DotCruz.Garage.Domain.ValueObjects;

namespace DotCruz.Garage.Application.Abstractions.Services.Security;

public interface ICheckinTokenService
{
    string GenerateToken(Guid vehicleId, Guid workspaceId, TimeSpan? validity = null);
    bool TryValidateToken(string tokenString, out OdometerCheckinToken? token);

    string GeneratePostSaleToken(Guid vehicleId, Guid saleId, Guid workspaceId, TimeSpan? validity = null);
    bool TryValidatePostSaleToken(string tokenString, out PostSalePortalToken? token);
}
