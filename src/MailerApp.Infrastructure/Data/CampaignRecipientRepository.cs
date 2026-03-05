using MailerApp.Domain.Entities;
using MailerApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailerApp.Infrastructure.Data;

public class CampaignRecipientRepository : ICampaignRecipientRepository
{
    private readonly MailerDbContext _db;

    public CampaignRecipientRepository(MailerDbContext db) => _db = db;

    public async Task<CampaignRecipient?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.CampaignRecipients.Include(r => r.Contact).Include(r => r.Campaign).FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<CampaignRecipient> recipients, CancellationToken cancellationToken = default)
    {
        _db.CampaignRecipients.AddRange(recipients);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(CampaignRecipient recipient, CancellationToken cancellationToken = default)
    {
        _db.CampaignRecipients.Update(recipient);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
