using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

using Server.Infrastructure.Data;
using Server.Infrastructure.Data.Seeding;

namespace Server.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    ///     Executa o seed do banco de dados
    /// </summary>
    public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        try
        {
            ServerDbContext context = services.GetRequiredService<ServerDbContext>();
            DatabaseSeeder seeder = services.GetRequiredService<DatabaseSeeder>();

            // 1. 🗄️ Executar as migrations
            await context.Database.MigrateAsync();

            // 2. 🌱 Executar seeds automático
            await seeder.SeedAllAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "❌ An error occurred while seeding the database");
            throw;
        }

        return app;
    }
}
