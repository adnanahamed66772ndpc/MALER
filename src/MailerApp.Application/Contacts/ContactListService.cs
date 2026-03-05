using MailerApp.Domain.Entities;
using MailerApp.Domain.Interfaces;

namespace MailerApp.Application.Contacts;

public class ContactListService
{
    private readonly IContactListRepository _lists;
    private readonly IContactRepository _contacts;

    public ContactListService(IContactListRepository lists, IContactRepository contacts)
    {
        _lists = lists;
        _contacts = contacts;
    }

    public async Task<IReadOnlyList<ContactListDto>> GetAllListsAsync(CancellationToken cancellationToken = default)
    {
        var list = await _lists.GetAllAsync(cancellationToken);
        return list.Select(l => new ContactListDto(l.Id, l.Name, l.Description, l.CreatedAt)).ToList();
    }

    public async Task<ContactListDto?> GetListByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var l = await _lists.GetByIdAsync(id, cancellationToken);
        return l == null ? null : new ContactListDto(l.Id, l.Name, l.Description, l.CreatedAt);
    }

    public async Task<ContactListDto> CreateListAsync(string name, string? description, CancellationToken cancellationToken = default)
    {
        var list = new ContactList { Name = name, Description = description };
        var added = await _lists.AddAsync(list, cancellationToken);
        return new ContactListDto(added.Id, added.Name, added.Description, added.CreatedAt);
    }

    public async Task<IReadOnlyList<ContactDto>> GetContactsByListAsync(int listId, CancellationToken cancellationToken = default)
    {
        var contacts = await _contacts.GetByListIdAsync(listId, cancellationToken);
        return contacts.Select(c => new ContactDto(c.Id, c.Email, c.FirstName, c.LastName, c.Company, c.CreatedAt)).ToList();
    }
}
