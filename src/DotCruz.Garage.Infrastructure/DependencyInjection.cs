using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Services.Ai;
using DotCruz.Garage.Application.Abstractions.Services.Auth;
using DotCruz.Garage.Application.Abstractions.Services.Notifications;
using DotCruz.Garage.Application.Abstractions.Services.Storage;
using DotCruz.Garage.Infrastructure.Data;
using DotCruz.Garage.Infrastructure.Data.Repositories.Maintenance;
using DotCruz.Garage.Infrastructure.Data.Repositories.ServiceOrders;
using DotCruz.Garage.Infrastructure.Data.Repositories.Vehicles;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Services.ManualExtraction;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Services.ServiceOrderExtraction;
using DotCruz.Garage.Infrastructure.Services.Auth;
using DotCruz.Garage.Infrastructure.Services.Notifications;
using DotCruz.Garage.Infrastructure.Services.Storage;
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
        services.AddScoped<IVehicleCatalogRepository, VehicleCatalogRepository>();
        services.AddScoped<IVehicleBlueprintRepository, VehicleBlueprintRepository>();

        services.AddScoped<IMaintenanceScheduleReadRepository, MaintenanceScheduleReadRepository>();
        services.AddScoped<IMaintenanceScheduleWriteRepository, MaintenanceScheduleWriteRepository>();

        services.AddScoped<IServiceOrderReadRepository, ServiceOrderReadRepository>();
        services.AddScoped<IServiceOrderWriteRepository, ServiceOrderWriteRepository>();
    }

    private static void AddUnitOfWork(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
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

        services.Configure<CloudflareR2Settings>(configuration.GetSection(CloudflareR2Settings.SectionName));
        services.AddScoped<IStorageService, CloudflareR2StorageService>();

        services.AddScoped<IAiManualExtractionService, GeminiAiManualExtractionService>();
        services.AddHttpClient<IAiServiceOrderExtractionService, GeminiAiServiceOrderExtractionService>();

        services.AddHttpClient<IGeminiClient, GeminiClient>(client =>
        {
            var baseUrl = configuration["Settings:GeminiApi:BaseUrl"];
            var apiKey = configuration["Settings:GeminiApi:ApiKey"];
            
            client.BaseAddress = new Uri(string.IsNullOrWhiteSpace(baseUrl) 
                ? "https://generativelanguage.googleapis.com/v1beta/" 
                : baseUrl);

            if (!string.IsNullOrWhiteSpace(apiKey))
                client.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

            client.DefaultRequestHeaders.Add("Api-Revision", "2026-05-20");
        });
    }
}
