using MailerApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailerApp.Infrastructure.Data.Configurations;

public class SendJobConfiguration : IEntityTypeConfiguration<SendJob>
{
    public void Configure(EntityTypeBuilder<SendJob> builder)
    {
        builder.ToTable("SendJobs");
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.Campaign).WithMany().HasForeignKey(e => e.CampaignId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.CampaignRecipient).WithMany().HasForeignKey(e => e.CampaignRecipientId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.SenderAccount).WithMany().HasForeignKey(e => e.SenderAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}
