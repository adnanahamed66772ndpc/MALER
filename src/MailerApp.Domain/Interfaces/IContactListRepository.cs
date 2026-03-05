using MailerApp.Domain.Entities;

namespace MailerApp.Domain.Interfaces;

public interface IContactListRepository
{
    Task<ContactList?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactList>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ContactList> AddAsync(ContactList list, CancellationToken cancellationToken = default);
    Task UpdateAsync(ContactList list, CancellationToken cancellationToken = default);
    Task AddMemberAsync(int contactId, int listId, CancellationToken cancellationToken = default);
    Task AddMembersAsync(int listId, IEnumerable<int> contactIds, CancellationToken cancellationToken = default);
    Task RemoveMemberAsync(int contactId, int listId, CancellationToken cancellationToken = default);
}
