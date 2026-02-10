using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Server.Domain.Entities;

namespace Server.Infrastructure.Data.Configuration;

public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        // Configuração da Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedNever(); // Guid é gerado no construtor

        // Configuração de CreatedAt - PostgreSQL
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()"); // PostgreSQL function

        // Configuração de UpdatedAt - PostgreSQL
        builder.Property(x => x.UpdatedAt)
            .IsRequired(false)
            .HasColumnType("timestamptz");

        // Configuração de DeletedAt (SoftDelete) - PostgreSQL
        builder.Property(x => x.DeletedAt)
            .IsRequired(false)
            .HasColumnType("timestamptz");

        // Configuração de IsActive
        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Índices para otimização - PostgreSQL naming convention
        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName($"ix_{typeof(T).Name.ToLower()}_created_at");

        builder.HasIndex(x => x.DeletedAt)
            .HasDatabaseName($"ix_{typeof(T).Name.ToLower()}_deleted_at");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName($"ix_{typeof(T).Name.ToLower()}_is_active");

        // Global Query Filter - só retorna registros não deletados
        builder.HasQueryFilter(x => x.DeletedAt == null);

        // Ignorar a propriedade computed IsDeleted no mapeamento
        builder.Ignore(x => x.IsDeleted);
    }
}
