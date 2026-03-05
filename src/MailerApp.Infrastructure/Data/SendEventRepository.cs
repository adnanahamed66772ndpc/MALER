using MailerApp.Domain.Entities;
using MailerApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailerApp.Infrastructure.Data;

public class SendEventRepository : ISendEventRepository
{
    private readonly MailerDbContext _db;

    public SendEventRepository(MailerDbContext db) => _db = db;

    public async Task<SendEvent> AddAsync(SendEvent sendEvent, CancellationToken cancellationToken = default)
    {
        sendEvent.OccurredAt = DateTime.UtcNow;
        _db.SendEvents.Add(sendEvent);
        await _db.SaveChangesAsync(cancellationToken);
        return sendEvent;
    }

    public async Task<IReadOnlyList<SendEvent>> GetByCampaignRecipientIdAsync(int campaignRecipientId, CancellationToken cancellationToken = default) =>
        await _db.SendEvents
            .Where(e => e.CampaignRecipientId == campaignRecipientId)
            .OrderBy(e => e.OccurredAt)
            .ToListAsync(cancellationToken);
}
