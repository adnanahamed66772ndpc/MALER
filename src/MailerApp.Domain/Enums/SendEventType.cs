namespace MailerApp.Domain.Enums;

public enum SendEventType
{
    Sent = 0,
    Delivered = 1,
    Open = 2,
    Click = 3,
    Bounce = 4,
    Complaint = 5,
    Unsubscribe = 6
}
