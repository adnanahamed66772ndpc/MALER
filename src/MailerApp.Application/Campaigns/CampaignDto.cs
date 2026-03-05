using MailerApp.Domain.Enums;

namespace MailerApp.Application.Campaigns;

public record CampaignDto(
    int Id,
    string Name,
    int TemplateId,
    string TemplateName,
    int SenderAccountId,
    string SenderEmail,
    CampaignStatus Status,
    int DailyCap,
    int DelayMsMin,
    int DelayMsMax,
    DateTime? ScheduledAt,
    DateTime? StartedAt,
    DateTime? EndedAt,
    DateTime CreatedAt,
    int TotalRecipients,
    int SentCount,
    int FailedCount
);
