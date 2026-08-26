using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Services.Security;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.GenerateOdometerCheckinToken;

public class GenerateOdometerCheckinTokenCommandHandler : IRequestHandler<GenerateOdometerCheckinTokenCommand, GenerateOdometerCheckinTokenResponseDto>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly ICheckinTokenService _tokenService;
    private readonly ITenantProvider _tenantProvider;
    private readonly IConfiguration _configuration;

    public GenerateOdometerCheckinTokenCommandHandler(
        IVehicleReadRepository vehicleReadRepository,
        ICheckinTokenService tokenService,
        ITenantProvider tenantProvider,
        IConfiguration configuration)
    {
        _vehicleReadRepository = vehicleReadRepository;
        _tokenService = tokenService;
        _tenantProvider = tenantProvider;
        _configuration = configuration;
    }

    public async Task<GenerateOdometerCheckinTokenResponseDto> Handle(GenerateOdometerCheckinTokenCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleReadRepository.GetByIdAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var tenantId = _tenantProvider.TenantId ?? vehicle.TenantId;
        var validity = request.Validity ?? TimeSpan.FromHours(48);
        var token = _tokenService.GenerateToken(vehicle.Id, tenantId, validity);

        var baseUrl = _configuration["Settings:FrontendUrl"] ?? "https://garage.dotcruz.com";
        var magicLinkUrl = $"{baseUrl.TrimEnd('/')}/checkin/odometer?token={token}";

        return new GenerateOdometerCheckinTokenResponseDto(
            vehicle.Id,
            token,
            magicLinkUrl,
            DateTimeOffset.UtcNow.Add(validity));
    }
}
