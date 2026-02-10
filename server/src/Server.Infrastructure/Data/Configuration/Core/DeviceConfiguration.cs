using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Server.Domain.Entities.Core;

namespace Server.Infrastructure.Data.Configuration.Core;

public class DeviceConfiguration : BaseEntityConfiguration<Device>
{
    public override void Configure(EntityTypeBuilder<Device> builder)
    {
        base.Configure(builder);

        builder.ToTable("devices");

        builder.Property(d => d.Brand)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Color)
            .HasMaxLength(50);

        builder.Property(d => d.SerialNumber)
            .HasMaxLength(100);

        builder.Property(d => d.Imei)
            .HasMaxLength(15);

        builder.Property(d => d.Condition)
            .HasMaxLength(500);

        // Relacionamentos
        builder.HasOne(d => d.Client)
            .WithMany(c => c.Devices)
            .HasForeignKey(d => d.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indices
        builder.HasIndex(d => d.ClientId)
            .HasDatabaseName("ix_devices_client_id");

        builder.HasIndex(d => d.Imei)
            .HasDatabaseName("ix_devices_imei");
    }
}
