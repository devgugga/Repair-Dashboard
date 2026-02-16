using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

using Serilog;

using Server.Api.Authorization;
using Server.Api.Filters;
using Server.Api.Middlewares;
using Server.Application;
using Server.Domain.ValueObjects.Options.Security;
using Server.Infrastructure;
using Server.Infrastructure.Extensions;
using Server.Infrastructure.Utils.Logging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
const string corsPolicyName = "ClientCors";

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

// Configure JWT bearer authentication for [Authorize] endpoints.
JwtOptions jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ??
                        throw new InvalidOperationException("Security:Jwt configuration is required.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = jwtOptions.ValidateIssuer,
            ValidateAudience = jwtOptions.ValidateAudience,
            ValidateLifetime = jwtOptions.ValidateLifetime,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew = TimeSpan.FromMinutes(jwtOptions.ClockSkewMinutes)
        };
    });

// Configure CORS policy used by browser clients (frontend).
builder.Services.AddCors(options =>
{
    string[] origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
                       Array.Empty<string>();

    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

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
app.UseCors(corsPolicyName);
// Authentication must run before Authorization.
app.UseAuthentication();
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
