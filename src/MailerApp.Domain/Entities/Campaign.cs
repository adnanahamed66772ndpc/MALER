using MailerApp.Domain.Enums;

namespace MailerApp.Domain.Entities;

public class Campaign
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TemplateId { get; set; }
    public Template Template { get; set; } = null!;
    public int SenderAccountId { get; set; }
    public SenderAccount SenderAccount { get; set; } = null!;
    public CampaignStatus Status { get; set; } = CampaignStatus.Draft;
    public int DailyCap { get; set; }
    public int DelayMsMin { get; set; }
    public int DelayMsMax { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<CampaignRecipient> Recipients { get; set; } = new List<CampaignRecipient>();
    /// <summary>Optional: multiple senders with per-sender caps (e.g. 250 from first, 200 from second). When empty, all sends use SenderAccountId.</summary>
    public ICollection<CampaignSender> Senders { get; set; } = new List<CampaignSender>();
}
