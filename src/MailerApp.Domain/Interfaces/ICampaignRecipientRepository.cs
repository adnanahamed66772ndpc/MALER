using MailerApp.Domain.Entities;

namespace MailerApp.Domain.Interfaces;

public interface ICampaignRecipientRepository
{
    Task<CampaignRecipient?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<CampaignRecipient> recipients, CancellationToken cancellationToken = default);
    Task UpdateAsync(CampaignRecipient recipient, CancellationToken cancellationToken = default);
}
