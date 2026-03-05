using MailerApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailerApp.Infrastructure.Data.Configurations;

public class SuppressionEntryConfiguration : IEntityTypeConfiguration<SuppressionEntry>
{
    public void Configure(EntityTypeBuilder<SuppressionEntry> builder)
    {
        builder.ToTable("SuppressionList");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(e => e.Email).IsUnique();
        builder.Property(e => e.Source).HasMaxLength(200);
    }
}
