namespace MailerApp.Domain.Entities;

public class ContactListMember
{
    public int ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
    public int ListId { get; set; }
    public ContactList List { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
}
