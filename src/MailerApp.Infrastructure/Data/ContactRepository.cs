using MailerApp.Domain.Entities;
using MailerApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailerApp.Infrastructure.Data;

public class ContactRepository : IContactRepository
{
    private readonly MailerDbContext _db;

    public ContactRepository(MailerDbContext db) => _db = db;

    public async Task<Contact?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.Contacts.FindAsync([id], cancellationToken);

    public async Task<Contact?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _db.Contacts.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

    public async Task<IReadOnlyList<Contact>> GetByListIdAsync(int listId, CancellationToken cancellationToken = default) =>
        await _db.ContactListMembers
            .Where(m => m.ListId == listId)
            .Include(m => m.Contact)
            .Select(m => m.Contact)
            .ToListAsync(cancellationToken);

    public async Task<Contact> AddAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        contact.CreatedAt = DateTime.UtcNow;
        _db.Contacts.Add(contact);
        await _db.SaveChangesAsync(cancellationToken);
        return contact;
    }

    public async Task AddRangeAsync(IEnumerable<Contact> contacts, CancellationToken cancellationToken = default)
    {
        var list = contacts.ToList();
        var now = DateTime.UtcNow;
        foreach (var c in list)
            c.CreatedAt = now;
        _db.Contacts.AddRange(list);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        _db.Contacts.Update(contact);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetExistingEmailsAsync(IEnumerable<string> emails, CancellationToken cancellationToken = default)
    {
        var set = emails.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return await _db.Contacts.Where(c => set.Contains(c.Email)).Select(c => c.Email).ToListAsync(cancellationToken);
    }
}
