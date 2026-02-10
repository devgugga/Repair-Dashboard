using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Server.Domain.Entities.Core;

namespace Server.Infrastructure.Data.Configuration.Core;

public class ClientConfiguration : BaseEntityConfiguration<Client>
{
    public override void Configure(EntityTypeBuilder<Client> builder)
    {
        base.Configure(builder);

        // Configurações da tabela
        builder.ToTable("clients");

        builder.Property(c => c.RepairedAmount)
            .IsRequired();

        // Índices
        builder.HasIndex(c => c.PersonId)
            .IsUnique() // 1:1 relationship
            .HasDatabaseName("ix_clients_person_id");
    }
}

