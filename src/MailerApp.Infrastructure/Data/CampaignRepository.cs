using MailerApp.Domain.Entities;
using MailerApp.Domain.Enums;
using MailerApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailerApp.Infrastructure.Data;

public class CampaignRepository : ICampaignRepository
{
    private readonly MailerDbContext _db;

    public CampaignRepository(MailerDbContext db) => _db = db;

    public async Task<Campaign?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.Campaigns
            .Include(c => c.Template)
            .Include(c => c.SenderAccount)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<Campaign?> GetByIdWithRecipientsAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.Campaigns
            .Include(c => c.Template)
            .Include(c => c.SenderAccount)
            .Include(c => c.Recipients).ThenInclude(r => r.Contact)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<Campaign?> GetByIdWithSendersAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.Campaigns
            .Include(c => c.Template)
            .Include(c => c.SenderAccount)
            .Include(c => c.Senders).ThenInclude(s => s.SenderAccount)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task SetSendersAsync(int campaignId, IReadOnlyList<(int SenderAccountId, int MaxEmails)> senderCaps, CancellationToken cancellationToken = default)
    {
        var existing = await _db.CampaignSenders.Where(s => s.CampaignId == campaignId).ToListAsync(cancellationToken);
        _db.CampaignSenders.RemoveRange(existing);
        for (var i = 0; i < senderCaps.Count; i++)
        {
            var (senderAccountId, maxEmails) = senderCaps[i];
            _db.CampaignSenders.Add(new CampaignSender { CampaignId = campaignId, SenderAccountId = senderAccountId, MaxEmails = maxEmails, SortOrder = i });
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Campaign>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.Campaigns
            .Include(c => c.Template)
            .Include(c => c.SenderAccount)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Campaign>> GetByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default) =>
        await _db.Campaigns
            .Include(c => c.Template)
            .Include(c => c.SenderAccount)
            .Where(c => c.Status == status)
            .ToListAsync(cancellationToken);

    public async Task<Campaign> AddAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        campaign.CreatedAt = DateTime.UtcNow;
        _db.Campaigns.Add(campaign);
        await _db.SaveChangesAsync(cancellationToken);
        return campaign;
    }

    public async Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        _db.Campaigns.Update(campaign);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
