using MailerApp.Domain.Enums;

namespace MailerApp.Domain.Entities;

public class SendJob
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public int CampaignRecipientId { get; set; }
    public CampaignRecipient CampaignRecipient { get; set; } = null!;
    /// <summary>When set, use this sender for this job; otherwise use campaign default sender.</summary>
    public int? SenderAccountId { get; set; }
    public SenderAccount? SenderAccount { get; set; }
    public SendJobStatus Status { get; set; } = SendJobStatus.Pending;
    public DateTime QueuedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public int Attempts { get; set; }
}
