namespace MailerApp.Application.Templates;

public record TemplateDto(int Id, string Name, string Subject, string? BodyHtml, string? BodyText, DateTime CreatedAt, DateTime UpdatedAt);
