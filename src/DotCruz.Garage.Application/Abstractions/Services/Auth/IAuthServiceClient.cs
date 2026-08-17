using DotCruz.Garage.Application.Abstractions.Services.Auth.DTOs;

namespace DotCruz.Garage.Application.Abstractions.Services.Auth;

public interface IAuthServiceClient
{
    Task<UserResponseDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
}
