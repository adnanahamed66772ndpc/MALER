namespace MailerApp.Application.Contacts;

public record ContactDto(int Id, string Email, string? FirstName, string? LastName, string? Company, DateTime CreatedAt);
