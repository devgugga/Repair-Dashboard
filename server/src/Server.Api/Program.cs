using Serilog;

using Server.Api.Filters;
using Server.Api.Middlewares;
using Server.Application;
using Server.Infrastructure;
using Server.Infrastructure.Extensions;
using Server.Infrastructure.Utils.Logging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure Serilog Early
builder.ConfigureSerilog();

builder.Services.AddHttpContextAccessor(); // Required for TraceService

builder.Services.AddControllers(options =>
{
    // Add global exception filter
    options.Filters.Add<GlobalExceptionFilter>();
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Dependencies
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

WebApplication app = builder.Build();

// Auto Migration and Database Seeding
if (app.Configuration.GetValue<bool>("System:Flags:AutoMigrations"))
{
    Log.Information("Auto-migrations enabled, seeding database...");
    await app.SeedDatabaseAsync();
}


// Add error handling middleware first (to catch all errors)
app.UseMiddleware<ErrorHandlingMiddleware>();
// Add request logging middleware
app.UseMiddleware<RequestLoggingMiddleware>();

// Configure the HTTP request pipeline.
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
