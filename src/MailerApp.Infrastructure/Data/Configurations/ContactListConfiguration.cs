using MailerApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailerApp.Infrastructure.Data.Configurations;

public class ContactListConfiguration : IEntityTypeConfiguration<ContactList>
{
    public void Configure(EntityTypeBuilder<ContactList> builder)
    {
        builder.ToTable("ContactLists");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(1000);
    }
}
