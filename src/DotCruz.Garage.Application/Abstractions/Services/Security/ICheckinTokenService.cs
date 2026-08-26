using DotCruz.Garage.Domain.ValueObjects;

namespace DotCruz.Garage.Application.Abstractions.Services.Security;

public interface ICheckinTokenService
{
    string GenerateToken(Guid vehicleId, Guid tenantId, TimeSpan? validity = null);
    bool TryValidateToken(string tokenString, out OdometerCheckinToken? token);
}
