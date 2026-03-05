using MailerApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailerApp.Infrastructure.Data.Configurations;

public class ContactListMemberConfiguration : IEntityTypeConfiguration<ContactListMember>
{
    public void Configure(EntityTypeBuilder<ContactListMember> builder)
    {
        builder.ToTable("ContactListMembers");
        builder.HasKey(e => new { e.ContactId, e.ListId });
        builder.HasOne(e => e.Contact).WithMany().HasForeignKey(e => e.ContactId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.List).WithMany(e => e.Members).HasForeignKey(e => e.ListId).OnDelete(DeleteBehavior.Cascade);
    }
}
