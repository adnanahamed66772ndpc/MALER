using MailerApp.Domain.Entities;

namespace MailerApp.Domain.Interfaces;

public interface ISenderAccountRepository
{
    Task<SenderAccount?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SenderAccount>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SenderAccount> AddAsync(SenderAccount account, CancellationToken cancellationToken = default);
    Task UpdateAsync(SenderAccount account, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
