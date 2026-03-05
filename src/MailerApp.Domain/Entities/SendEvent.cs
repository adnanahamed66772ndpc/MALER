using MailerApp.Domain.Enums;

namespace MailerApp.Domain.Entities;

public class SendEvent
{
    public int Id { get; set; }
    public int? SendJobId { get; set; }
    public int? CampaignRecipientId { get; set; }
    public SendEventType EventType { get; set; }
    public string? PayloadJson { get; set; }
    public DateTime OccurredAt { get; set; }
}
