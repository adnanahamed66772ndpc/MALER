namespace MailerApp.Application.Templates;

public record CreateTemplateCommand(string Name, string Subject, string? BodyHtml, string? BodyText);
