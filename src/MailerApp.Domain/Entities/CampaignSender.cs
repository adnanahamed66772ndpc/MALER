namespace MailerApp.Domain.Entities;

/// <summary>Assigns a sender account to a campaign with a max number of emails to send from that account.</summary>
public class CampaignSender
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public int SenderAccountId { get; set; }
    public SenderAccount SenderAccount { get; set; } = null!;
    /// <summary>Max emails to send from this sender for this campaign; after that the next sender is used.</summary>
    public int MaxEmails { get; set; }
    public int SortOrder { get; set; }
}
