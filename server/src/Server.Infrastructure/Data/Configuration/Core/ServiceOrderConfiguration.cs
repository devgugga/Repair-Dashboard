using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Server.Domain.Entities.Core;

namespace Server.Infrastructure.Data.Configuration.Core;

public class ServiceOrderConfiguration : BaseEntityConfiguration<ServiceOrder>
{
    public override void Configure(EntityTypeBuilder<ServiceOrder> builder)
    {
        base.Configure(builder);

        builder.ToTable("service_orders");

        builder.Property(so => so.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(so => so.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(so => so.Priority)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(so => so.EntryDate)
            .IsRequired()
            .HasColumnType("timestamptz");

        builder.Property(so => so.EstimatedCompletionDate)
            .HasColumnType("timestamptz");

        builder.Property(so => so.CompletionDate)
            .HasColumnType("timestamptz");

        builder.Property(so => so.DefectDescription)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(so => so.TechnicianDiagnosis)
            .HasMaxLength(1000);

        builder.Property(so => so.InternalNotes)
            .HasMaxLength(2000);

        // Relacionamentos
        builder.HasOne(so => so.Client)
            .WithMany(c => c.ServiceOrders)
            .HasForeignKey(so => so.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(so => so.Device)
            .WithMany(d => d.ServiceOrders)
            .HasForeignKey(so => so.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(so => so.Technician)
            .WithMany(u => u.AssignedServiceOrders)
            .HasForeignKey(so => so.TechnicianId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indices
        builder.HasIndex(so => so.Code)
            .IsUnique()
            .HasDatabaseName("ix_service_orders_code");

        builder.HasIndex(so => so.Status)
            .HasDatabaseName("ix_service_orders_status");

        builder.HasIndex(so => so.ClientId)
            .HasDatabaseName("ix_service_orders_client_id");

        builder.HasIndex(so => so.DeviceId)
            .HasDatabaseName("ix_service_orders_device_id");

        builder.HasIndex(so => so.TechnicianId)
            .HasDatabaseName("ix_service_orders_technician_id");
    }
}
