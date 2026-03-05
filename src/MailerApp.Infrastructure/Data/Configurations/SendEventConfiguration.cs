using MailerApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailerApp.Infrastructure.Data.Configurations;

public class SendEventConfiguration : IEntityTypeConfiguration<SendEvent>
{
    public void Configure(EntityTypeBuilder<SendEvent> builder)
    {
        builder.ToTable("SendEvents");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.PayloadJson);
    }
}
