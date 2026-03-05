using MailerApp.Domain.Entities;
using MailerApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailerApp.Infrastructure.Data;

public class ContactListRepository : IContactListRepository
{
    private readonly MailerDbContext _db;

    public ContactListRepository(MailerDbContext db) => _db = db;

    public async Task<ContactList?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.ContactLists.Include(l => l.Members).FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ContactList>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.ContactLists.OrderBy(l => l.Name).ToListAsync(cancellationToken);

    public async Task<ContactList> AddAsync(ContactList list, CancellationToken cancellationToken = default)
    {
        list.CreatedAt = DateTime.UtcNow;
        _db.ContactLists.Add(list);
        await _db.SaveChangesAsync(cancellationToken);
        return list;
    }

    public async Task UpdateAsync(ContactList list, CancellationToken cancellationToken = default)
    {
        _db.ContactLists.Update(list);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddMemberAsync(int contactId, int listId, CancellationToken cancellationToken = default)
    {
        if (await _db.ContactListMembers.AnyAsync(m => m.ContactId == contactId && m.ListId == listId, cancellationToken))
            return;
        _db.ContactListMembers.Add(new ContactListMember { ContactId = contactId, ListId = listId, JoinedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddMembersAsync(int listId, IEnumerable<int> contactIds, CancellationToken cancellationToken = default)
    {
        var existing = await _db.ContactListMembers.Where(m => m.ListId == listId).Select(m => m.ContactId).ToHashSetAsync(cancellationToken);
        var now = DateTime.UtcNow;
        foreach (var cid in contactIds.Distinct())
        {
            if (existing.Contains(cid)) continue;
            _db.ContactListMembers.Add(new ContactListMember { ContactId = cid, ListId = listId, JoinedAt = now });
            existing.Add(cid);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveMemberAsync(int contactId, int listId, CancellationToken cancellationToken = default)
    {
        var m = await _db.ContactListMembers.FirstOrDefaultAsync(x => x.ContactId == contactId && x.ListId == listId, cancellationToken);
        if (m != null)
        {
            _db.ContactListMembers.Remove(m);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
