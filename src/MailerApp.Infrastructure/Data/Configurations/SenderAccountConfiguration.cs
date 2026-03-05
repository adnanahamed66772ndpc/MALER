using MailerApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailerApp.Infrastructure.Data.Configurations;

public class SenderAccountConfiguration : IEntityTypeConfiguration<SenderAccount>
{
    public void Configure(EntityTypeBuilder<SenderAccount> builder)
    {
        builder.ToTable("SenderAccounts");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
        builder.Property(e => e.EncryptedRefreshToken);
        builder.Property(e => e.EncryptedAppPassword);
    }
}
