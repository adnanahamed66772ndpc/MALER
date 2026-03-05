using MailerApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MailerApp.Infrastructure.Data;

public class MailerDbContext : DbContext
{
    public MailerDbContext(DbContextOptions<MailerDbContext> options) : base(options) { }

    public DbSet<SenderAccount> SenderAccounts => Set<SenderAccount>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<ContactList> ContactLists => Set<ContactList>();
    public DbSet<ContactListMember> ContactListMembers => Set<ContactListMember>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignSender> CampaignSenders => Set<CampaignSender>();
    public DbSet<CampaignRecipient> CampaignRecipients => Set<CampaignRecipient>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<SendJob> SendJobs => Set<SendJob>();
    public DbSet<SendEvent> SendEvents => Set<SendEvent>();
    public DbSet<SuppressionEntry> SuppressionList => Set<SuppressionEntry>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MailerDbContext).Assembly);
    }
}
