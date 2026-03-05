namespace MailerApp.Application.Contacts;

public record ContactListDto(int Id, string Name, string? Description, DateTime CreatedAt);
