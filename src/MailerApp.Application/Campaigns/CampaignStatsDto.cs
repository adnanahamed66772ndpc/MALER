namespace MailerApp.Application.Campaigns;

public record CampaignStatsDto(int Sent, int Failed, int Bounced, int Replied, int Pending);
