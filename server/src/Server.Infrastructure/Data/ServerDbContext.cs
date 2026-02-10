using System.Reflection;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Server.Infrastructure.Data;

public class ServerDbContext(DbContextOptions<ServerDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações automaticamente
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configurações globais para PostgreSQL
        ConfigurePostgreSqlConventions(modelBuilder);
    }

    /// <summary>
    ///     Configurações específicas para PostgreSQL
    /// </summary>
    private static void ConfigurePostgreSqlConventions(ModelBuilder modelBuilder)
    {
        // Configuração global para strings
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Configurar nome da tabela em snake_case (convenção PostgreSQL)
            string? tableName = entityType.GetTableName();
            if (!string.IsNullOrEmpty(tableName)) entityType.SetTableName(ToSnakeCase(tableName));

            foreach (IMutableProperty property in entityType.GetProperties())
            {
                // Configurar nomes de colunas em snake_case
                string columnName = property.GetColumnName();
                if (!string.IsNullOrEmpty(columnName)) property.SetColumnName(ToSnakeCase(columnName));

                // Strings como VARCHAR ao invés de TEXT quando não especificado
                if (property.ClrType == typeof(string) && !property.GetMaxLength().HasValue) property.SetMaxLength(500);

                // Configuração para decimais com precisão
                if (property.ClrType != typeof(decimal) && property.ClrType != typeof(decimal?)) continue;

                property.SetPrecision(18);
                property.SetScale(2);
            }
        }
    }

    /// <summary>
    ///     Converte PascalCase para snake_case (convenção PostgreSQL)
    /// </summary>
    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new StringBuilder();
        result.Append(char.ToLower(input[0]));

        for (int i = 1; i < input.Length; i++)
            if (char.IsUpper(input[i]))
            {
                result.Append('_');
                result.Append(char.ToLower(input[i]));
            }
            else
            {
                result.Append(input[i]);
            }

        return result.ToString();
    }
}
