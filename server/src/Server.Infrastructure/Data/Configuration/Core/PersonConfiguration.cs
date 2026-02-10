using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Server.Domain.Entities.Core;

namespace Server.Infrastructure.Data.Configuration.Core;

public class PersonConfiguration : BaseEntityConfiguration<Person>
{
    public override void Configure(EntityTypeBuilder<Person> builder)
    {
        base.Configure(builder);

        // Configurações de propriedades
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Document)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ZipCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(p => p.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.AddressNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.AddressComplement)
            .HasMaxLength(255);

        // Enum para string
        builder.Property(p => p.DocumentType)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Índices únicos
        builder.HasIndex(p => p.Document)
            .IsUnique()
            .HasDatabaseName("ix_persons_document");

        builder.HasIndex(p => p.Email)
            .IsUnique()
            .HasDatabaseName("ix_persons_email");

        // Relacionamentos 1:1
        builder.HasOne(p => p.User)
            .WithOne(u => u.Person)
            .HasForeignKey<User>(u => u.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Client)
            .WithOne(c => c.Person)
            .HasForeignKey<Client>(c => c.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

