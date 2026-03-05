using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MailerApp.Infrastructure.Gmail;

public class GmailSmtpSender : MailerApp.Domain.Interfaces.IEmailSender
{
    private readonly string _fromEmail;
    private readonly string _appPassword;

    public GmailSmtpSender(string fromEmail, string appPassword)
    {
        _fromEmail = fromEmail;
        _appPassword = appPassword;
    }

    public async Task SendAsync(string fromAddress, string toAddress, string subject, string bodyHtml, string? bodyText, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(fromAddress));
        message.To.Add(MailboxAddress.Parse(toAddress));
        message.Subject = subject;
        var builder = new BodyBuilder { HtmlBody = bodyHtml };
        if (!string.IsNullOrEmpty(bodyText))
            builder.TextBody = bodyText;
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(_fromEmail, _appPassword, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
