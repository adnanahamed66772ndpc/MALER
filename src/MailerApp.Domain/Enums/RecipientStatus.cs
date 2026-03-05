namespace MailerApp.Domain.Enums;

public enum RecipientStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2,
    Bounced = 3,
    Unsubscribed = 4
}
