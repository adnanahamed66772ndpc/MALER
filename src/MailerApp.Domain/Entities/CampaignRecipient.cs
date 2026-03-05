using MailerApp.Domain.Enums;

namespace MailerApp.Domain.Entities;

public class CampaignRecipient
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public int ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
    public int? ListId { get; set; }
    public RecipientStatus Status { get; set; } = RecipientStatus.Pending;
    public DateTime? SentAt { get; set; }
    public string? ErrorCode { get; set; }
    public int RetryCount { get; set; }
}
