namespace MailerApp.Domain.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string fromAddress, string toAddress, string subject, string bodyHtml, string? bodyText, CancellationToken cancellationToken = default);
}
