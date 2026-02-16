using Microsoft.AspNetCore.Authorization;

using Serilog;

using Server.Api.Authorization;
using Server.Api.Filters;
using Server.Api.Middlewares;
using Server.Application;
using Server.Infrastructure;
using Server.Infrastructure.Extensions;
using Server.Infrastructure.Utils.Logging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure Serilog before the app pipeline starts.
builder.ConfigureSerilog();

builder.Services.AddHttpContextAccessor(); // Required for TraceService.

builder.Services.AddControllers(options =>
{
    // Register global exception filter.
    options.Filters.Add<GlobalExceptionFilter>();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register application dependencies.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Configure authorization components.
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

WebApplication app = builder.Build();

// Apply migrations and seed data automatically when feature flag is enabled.
if (app.Configuration.GetValue<bool>("System:Flags:AutoMigrations"))
{
    Log.Information("Auto-migrations enabled, seeding database...");
    await app.SeedDatabaseAsync();
}

// Error handling should run first so all downstream failures are captured.
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting Server application...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
