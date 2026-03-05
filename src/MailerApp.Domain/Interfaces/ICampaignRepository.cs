using MailerApp.Domain.Entities;
using MailerApp.Domain.Enums;

namespace MailerApp.Domain.Interfaces;

public interface ICampaignRepository
{
    Task<Campaign?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Campaign?> GetByIdWithRecipientsAsync(int id, CancellationToken cancellationToken = default);
    Task<Campaign?> GetByIdWithSendersAsync(int id, CancellationToken cancellationToken = default);
    Task SetSendersAsync(int campaignId, IReadOnlyList<(int SenderAccountId, int MaxEmails)> senderCaps, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Campaign>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Campaign>> GetByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default);
    Task<Campaign> AddAsync(Campaign campaign, CancellationToken cancellationToken = default);
    Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken = default);
}
