using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Server.Domain.Entities.Core;

namespace Server.Infrastructure.Data.Configuration.Core;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        // Configurações da tabela
        builder.ToTable("users");

        // Configurações de propriedades
        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        // Enum para string
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Índices
        builder.HasIndex(u => u.UserName)
            .IsUnique()
            .HasDatabaseName("ix_users_username");

        builder.HasIndex(u => u.PersonId)
            .IsUnique() // 1:1 relationship
            .HasDatabaseName("ix_users_person_id");
    }

}
