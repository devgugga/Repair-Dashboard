using Microsoft.Extensions.DependencyInjection;

namespace Server.Infrastructure.Data.Seeding;

public abstract class BaseDataSeeder : IDataSeeder
{
    public abstract int Order { get; }
    public abstract string Name { get; }

    public abstract Task SeedAsync(ServerDbContext context, IServiceProvider serviceProvider);
    public abstract Task<bool> HasBeenSeededAsync(ServerDbContext context);

    protected T GetRequiredService<T>(IServiceProvider serviceProvider) where T : notnull
    {
        return serviceProvider.GetRequiredService<T>();
    }

    protected T? GetService<T>(IServiceProvider serviceProvider) where T : class
    {
        return serviceProvider.GetService<T>();
    }
}
