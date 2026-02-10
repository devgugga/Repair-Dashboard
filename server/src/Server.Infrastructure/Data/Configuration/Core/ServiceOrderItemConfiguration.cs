using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Server.Domain.Entities.Core;

namespace Server.Infrastructure.Data.Configuration.Core;

public class ServiceOrderItemConfiguration : BaseEntityConfiguration<ServiceOrderItem>
{
    public override void Configure(EntityTypeBuilder<ServiceOrderItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("service_order_items");

        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(i => i.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(i => i.ServiceOrder)
            .WithMany(so => so.Items)
            .HasForeignKey(i => i.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indices
        builder.HasIndex(i => i.ServiceOrderId)
            .HasDatabaseName("ix_service_order_items_service_order_id");
    }
}
