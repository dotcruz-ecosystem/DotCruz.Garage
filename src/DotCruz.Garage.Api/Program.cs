using DotCruz.Garage.Api.Configurations;
using DotCruz.Garage.Api.Handlers;
using DotCruz.Garage.Api.Middlewares;
using DotCruz.Garage.Application;
using DotCruz.Garage.Infrastructure;
using DotCruz.Shared.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSharedSecurity(builder.Configuration);

builder.Services.AddExceptionHandler<GarageExceptionHandler>();
builder.Services.AddApiConventions();
builder.Services.AddOpenApiDocumentation();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.UseMiddleware<CultureMiddleware>();

app.MapOpenApiDocumentation();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
