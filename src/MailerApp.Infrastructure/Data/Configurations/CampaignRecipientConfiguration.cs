using MailerApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailerApp.Infrastructure.Data.Configurations;

public class CampaignRecipientConfiguration : IEntityTypeConfiguration<CampaignRecipient>
{
    public void Configure(EntityTypeBuilder<CampaignRecipient> builder)
    {
        builder.ToTable("CampaignRecipients");
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.Campaign).WithMany(e => e.Recipients).HasForeignKey(e => e.CampaignId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Contact).WithMany().HasForeignKey(e => e.ContactId).OnDelete(DeleteBehavior.Restrict);
    }
}
