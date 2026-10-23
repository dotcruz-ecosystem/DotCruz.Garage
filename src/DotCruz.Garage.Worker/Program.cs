using DotCruz.Garage.Application;
using DotCruz.Garage.Infrastructure;
using DotCruz.Garage.Worker.BackgroundServices;
using DotCruz.Shared.Security;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSharedSecurity(builder.Configuration);

builder.Services.AddHostedService<VehicleBlueprintPollerWorker>();

var host = builder.Build();
host.Run();
