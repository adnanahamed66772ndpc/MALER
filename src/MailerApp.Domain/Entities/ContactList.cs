namespace MailerApp.Domain.Entities;

public class ContactList
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<ContactListMember> Members { get; set; } = new List<ContactListMember>();
}
