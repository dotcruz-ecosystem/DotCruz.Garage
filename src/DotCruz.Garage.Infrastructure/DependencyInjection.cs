using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Accessories;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Catalog;
using DotCruz.Shared.Security.Context;
using DotCruz.Garage.Application.Abstractions.Services.Auth;
using DotCruz.Garage.Application.Abstractions.Services.Notifications;
using DotCruz.Garage.Application.Abstractions.Services.Ai;
using DotCruz.Garage.Infrastructure.Data;
using DotCruz.Garage.Infrastructure.Data.Repositories.Vehicles;
using DotCruz.Garage.Infrastructure.Data.Repositories.Maintenance;
using DotCruz.Garage.Infrastructure.Data.Repositories.Fines;
using DotCruz.Garage.Infrastructure.Data.Repositories.Documents;
using DotCruz.Garage.Infrastructure.Data.Repositories.Accessories;
using DotCruz.Garage.Infrastructure.Data.Repositories.Catalog;
using DotCruz.Garage.Infrastructure.Services;
using DotCruz.Garage.Infrastructure.Services.Auth;
using DotCruz.Garage.Infrastructure.Services.Notifications;
using DotCruz.Garage.Infrastructure.Services.Ai;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotCruz.Garage.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services, configuration);
        AddRepositories(services);
        AddUnitOfWork(services);
        AddServices(services, configuration);

        return services;
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<GarageDbContext>(opt =>
        {
            opt.UseNpgsql(connectionString);
            opt.UseSnakeCaseNamingConvention();
        });
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IVehicleReadRepository, VehicleReadRepository>();
        services.AddScoped<IVehicleWriteRepository, VehicleWriteRepository>();

        services.AddScoped<IMaintenanceScheduleReadRepository, MaintenanceScheduleReadRepository>();
        services.AddScoped<IMaintenanceScheduleWriteRepository, MaintenanceScheduleWriteRepository>();

        services.AddScoped<IMaintenanceLogReadRepository, MaintenanceLogReadRepository>();
        services.AddScoped<IMaintenanceLogWriteRepository, MaintenanceLogWriteRepository>();

        services.AddScoped<ITrafficFineReadRepository, TrafficFineReadRepository>();
        services.AddScoped<ITrafficFineWriteRepository, TrafficFineWriteRepository>();

        services.AddScoped<IVehicleDocumentReadRepository, VehicleDocumentReadRepository>();
        services.AddScoped<IVehicleDocumentWriteRepository, VehicleDocumentWriteRepository>();

        services.AddScoped<IVehicleAccessoryReadRepository, VehicleAccessoryReadRepository>();
        services.AddScoped<IVehicleAccessoryWriteRepository, VehicleAccessoryWriteRepository>();

        services.AddScoped<IVehicleBlueprintReadRepository, VehicleBlueprintReadRepository>();
        services.AddScoped<IVehicleBlueprintWriteRepository, VehicleBlueprintWriteRepository>();
    }

    private static void AddUnitOfWork(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IAiManualParserService, GeminiAiManualParserService>();

        services.AddHttpClient<INotificationService, NotificationService>(client =>
        {
            var baseUrl = configuration["Settings:NotificationApiUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                client.BaseAddress = new Uri(baseUrl);
            }
        })
        .AddServiceApiKeyPropagation();

        services.AddHttpClient<IAuthServiceClient, AuthServiceClient>(client =>
        {
            var baseUrl = configuration["Settings:AuthApiUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                client.BaseAddress = new Uri(baseUrl);
            }
        })
        .AddServiceApiKeyPropagation();
    }
}
