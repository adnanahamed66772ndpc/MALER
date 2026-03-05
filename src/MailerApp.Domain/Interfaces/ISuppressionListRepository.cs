using MailerApp.Domain.Entities;

namespace MailerApp.Domain.Interfaces;

public interface ISuppressionListRepository
{
    Task<bool> IsSuppressedAsync(string email, CancellationToken cancellationToken = default);
    Task<SuppressionEntry?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetSuppressedEmailsAsync(IEnumerable<string> emails, CancellationToken cancellationToken = default);
    Task<SuppressionEntry> AddAsync(SuppressionEntry entry, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SuppressionEntry>> GetAllAsync(int skip = 0, int take = 1000, CancellationToken cancellationToken = default);
}
