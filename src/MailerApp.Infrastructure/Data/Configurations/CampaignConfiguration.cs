using MailerApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailerApp.Infrastructure.Data.Configurations;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("Campaigns");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.HasOne(e => e.Template).WithMany().HasForeignKey(e => e.TemplateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.SenderAccount).WithMany().HasForeignKey(e => e.SenderAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}
