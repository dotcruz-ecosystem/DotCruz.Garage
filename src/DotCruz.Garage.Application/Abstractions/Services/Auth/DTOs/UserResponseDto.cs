namespace DotCruz.Garage.Application.Abstractions.Services.Auth.DTOs;

public sealed record UserResponseDto(
    Guid Id,
    string Name,
    string Email
);
