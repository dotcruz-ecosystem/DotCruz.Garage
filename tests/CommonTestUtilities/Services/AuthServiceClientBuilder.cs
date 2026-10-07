using DotCruz.Garage.Application.Abstractions.Services.Auth;
using DotCruz.Garage.Application.Abstractions.Services.Auth.DTOs;
using Moq;

namespace CommonTestUtilities.Services;

public class AuthServiceClientBuilder
{
    private readonly Mock<IAuthServiceClient> _client = new();

    public AuthServiceClientBuilder SetupGetUserById(Guid userId, Guid workspaceId, UserResponseDto? user)
    {
        _client
            .Setup(c => c.GetUserByIdAsync(userId, workspaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        return this;
    }

    public AuthServiceClientBuilder SetupAnyUser(UserResponseDto user)
    {
        _client
            .Setup(c => c.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        return this;
    }

    public Mock<IAuthServiceClient> Mock => _client;

    public IAuthServiceClient Build() => _client.Object;
}
