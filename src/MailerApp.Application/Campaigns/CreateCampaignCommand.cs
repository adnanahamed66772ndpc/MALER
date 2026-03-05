namespace MailerApp.Application.Campaigns;

public record CreateCampaignCommand(
    string Name,
    int TemplateId,
    int SenderAccountId,
    int DailyCap,
    int DelayMsMin,
    int DelayMsMax,
    DateTime? ScheduledAt
);
