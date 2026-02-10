namespace Server.Infrastructure.Data.Seeding;

public interface IDataSeeder
{
    /// <summary>
    ///     Ordem de execução do seeder (menor número executa primeiro)
    /// </summary>
    int Order { get; }

    /// <summary>
    ///     Nome do seeder para logs
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Executa o seed dos dados
    /// </summary>
    Task SeedAsync(ServerDbContext context, IServiceProvider serviceProvider);

    /// <summary>
    ///     Verifica se o seed já foi executado
    /// </summary>
    Task<bool> HasBeenSeededAsync(ServerDbContext context);
}
