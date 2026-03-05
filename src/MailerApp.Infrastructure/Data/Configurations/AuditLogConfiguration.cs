using MailerApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailerApp.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(e => e.EntityId).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Action).IsRequired().HasMaxLength(100);
        builder.Property(e => e.UserOrSystem).HasMaxLength(200);
    }
}
