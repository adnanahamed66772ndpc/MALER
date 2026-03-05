using MailerApp.Domain.Entities;

namespace MailerApp.Domain.Interfaces;

public interface ISendEventRepository
{
    Task<SendEvent> AddAsync(SendEvent sendEvent, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SendEvent>> GetByCampaignRecipientIdAsync(int campaignRecipientId, CancellationToken cancellationToken = default);
}
