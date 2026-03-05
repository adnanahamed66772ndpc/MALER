namespace MailerApp.Domain.Enums;

public enum SenderProvider
{
    GmailApi = 0,
    GmailSmtp = 1,
    // Phase 2: SendGrid, Ses, Mailgun
}
