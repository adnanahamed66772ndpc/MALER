using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using MailerApp.Domain.Entities;
using MailerApp.Domain.Interfaces;
using MimeKit;

namespace MailerApp.Infrastructure.Gmail;

public class GmailApiSender : IEmailSender
{
    private readonly GmailService _gmailService;
    private readonly string _fromEmail;

    public GmailApiSender(GmailService gmailService, string fromEmail)
    {
        _gmailService = gmailService;
        _fromEmail = fromEmail;
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
        using var stream = new MemoryStream();
        await message.WriteToAsync(stream, cancellationToken);
        var raw = ToBase64Url(stream.ToArray());
        var gmailMessage = new Message { Raw = raw };
        await _gmailService.Users.Messages.Send(gmailMessage, "me").ExecuteAsync(cancellationToken);
    }

    private static string ToBase64Url(byte[] input)
    {
        var base64 = System.Convert.ToBase64String(input);
        return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
}
