using MailerApp.Domain.Entities;
using MailerApp.Domain.Enums;

namespace MailerApp.Domain.Interfaces;

public interface ISendJobRepository
{
    Task<SendJob> AddAsync(SendJob job, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<SendJob> jobs, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SendJob>> DequeueAsync(int campaignId, int maxCount, CancellationToken cancellationToken = default);
    Task UpdateAsync(SendJob job, CancellationToken cancellationToken = default);
}
