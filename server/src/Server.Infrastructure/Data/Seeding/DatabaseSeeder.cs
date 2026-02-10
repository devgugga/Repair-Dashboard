using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Serilog;

namespace Server.Infrastructure.Data.Seeding;

public class DatabaseSeeder(
    ServerDbContext context,
    IServiceProvider serviceProvider)
{
    /// <summary>
    ///     Executa todos os seeders em ordem
    /// </summary>
    public async Task SeedAllAsync()
    {
        Log.Information("🚀 Starting database seeding process...");

        // Auto-discovery: scan assembly for all concrete BaseDataSeeder implementations
        var seeders = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(BaseDataSeeder)) && !t.IsAbstract)
            .Select(t => (IDataSeeder)Activator.CreateInstance(t)!)
            .OrderBy(s => s.Order)
            .ToList();

        int totalSeeders = seeders.Count;
        int completedSeeders = 0;

        // Obter execution strategy para compatibilidade com NpgsqlRetryingExecutionStrategy
        IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

        foreach (IDataSeeder seeder in seeders)
            try
            {
                Log.Information("📦 [{Current}/{Total}] Checking {SeederName}...",
                    completedSeeders + 1, totalSeeders, seeder.Name);

                // Verificar se já foi executado
                if (await seeder.HasBeenSeededAsync(context))
                {
                    Log.Information("⏭️  {SeederName} already executed, skipping...", seeder.Name);
                }
                else
                {
                    // Executar seeder dentro de transação via execution strategy
                    await strategy.ExecuteAsync(async () =>
                    {
                        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();
                        await seeder.SeedAsync(context, serviceProvider);
                        await transaction.CommitAsync();
                    });
                    Log.Information("✅ {SeederName} completed successfully", seeder.Name);
                }

                completedSeeders++;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ Error executing {SeederName}: {ErrorMessage}",
                    seeder.Name, ex.Message);
                throw; // Re-throw para parar o processo
            }

        Log.Information("🎊 Database seeding completed! {CompletedCount}/{TotalCount} seeders executed",
            completedSeeders, totalSeeders);
    }

    /// <summary>
    ///     Executa apenas um seeder específico
    /// </summary>
    public async Task SeedAsync<T>() where T : IDataSeeder, new()
    {
        var seeder = new T();

        Log.Information("🎯 Running specific seeder: {SeederName}", seeder.Name);

        if (await seeder.HasBeenSeededAsync(context))
        {
            Log.Warning("⚠️  {SeederName} already executed!", seeder.Name);
            return;
        }

        IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();
            await seeder.SeedAsync(context, serviceProvider);
            await transaction.CommitAsync();
        });
        Log.Information("✅ {SeederName} completed", seeder.Name);
    }
}
