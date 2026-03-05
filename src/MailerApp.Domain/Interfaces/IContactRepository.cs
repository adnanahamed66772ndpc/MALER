using MailerApp.Domain.Entities;

namespace MailerApp.Domain.Interfaces;

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Contact?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Contact>> GetByListIdAsync(int listId, CancellationToken cancellationToken = default);
    Task<Contact> AddAsync(Contact contact, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Contact> contacts, CancellationToken cancellationToken = default);
    Task UpdateAsync(Contact contact, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetExistingEmailsAsync(IEnumerable<string> emails, CancellationToken cancellationToken = default);
}
