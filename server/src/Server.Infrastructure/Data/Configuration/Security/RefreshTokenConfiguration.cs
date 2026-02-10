using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Server.Domain.Entities.Security;

namespace Server.Infrastructure.Data.Configuration.Security;

public class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        // Properties
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(rt => rt.IpAddress)
            .HasMaxLength(45); // IPv6 support

        builder.Property(rt => rt.UserAgent)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(rt => rt.Token)
            .IsUnique()
            .HasDatabaseName("ix_refresh_tokens_token");

        builder.HasIndex(rt => rt.UserId)
            .HasDatabaseName("ix_refresh_tokens_user_id");

        builder.HasIndex(rt => new { rt.UserId, rt.IsRevoked })
            .HasDatabaseName("ix_refresh_tokens_user_revoked");


        // Relationships
        builder.HasOne(rt => rt.User)
            .WithMany() // User não tem lista de refresh tokens por performance
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore computed properties
        builder.Ignore(rt => rt.IsExpired);
        builder.Ignore(rt => rt.IsActive);
    }
}
