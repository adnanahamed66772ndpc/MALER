using MailerApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailerApp.Infrastructure.Data.Configurations;

public class CampaignSenderConfiguration : IEntityTypeConfiguration<CampaignSender>
{
    public void Configure(EntityTypeBuilder<CampaignSender> builder)
    {
        builder.ToTable("CampaignSenders");
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.Campaign).WithMany(c => c.Senders).HasForeignKey(e => e.CampaignId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.SenderAccount).WithMany().HasForeignKey(e => e.SenderAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}
