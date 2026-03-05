using MailerApp.Domain.Entities;
using MailerApp.Domain.Enums;
using MailerApp.Domain.Interfaces;

namespace MailerApp.Application.Campaigns;

public class CampaignService
{
    private readonly ICampaignRepository _campaigns;
    private readonly ICampaignRecipientRepository _recipients;
    private readonly ISendJobRepository _sendJobs;
    private readonly IContactListRepository _lists;
    private readonly IContactRepository _contacts;
    private readonly ISuppressionListRepository _suppression;

    public CampaignService(
        ICampaignRepository campaigns,
        ICampaignRecipientRepository recipients,
        ISendJobRepository sendJobs,
        IContactListRepository lists,
        IContactRepository contacts,
        ISuppressionListRepository suppression)
    {
        _campaigns = campaigns;
        _recipients = recipients;
        _sendJobs = sendJobs;
        _lists = lists;
        _contacts = contacts;
        _suppression = suppression;
    }

    public async Task<CampaignDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var c = await _campaigns.GetByIdWithRecipientsAsync(id, cancellationToken);
        if (c == null) return null;
        var sent = c.Recipients.Count(r => r.Status == RecipientStatus.Sent);
        var failed = c.Recipients.Count(r => r.Status == RecipientStatus.Failed);
        return new CampaignDto(
            c.Id, c.Name, c.TemplateId, c.Template.Name, c.SenderAccountId, c.SenderAccount.Email,
            c.Status, c.DailyCap, c.DelayMsMin, c.DelayMsMax, c.ScheduledAt, c.StartedAt, c.EndedAt, c.CreatedAt,
            c.Recipients.Count, sent, failed);
    }

    public async Task<CampaignStatsDto?> GetCampaignStatsAsync(int campaignId, CancellationToken cancellationToken = default)
    {
        var c = await _campaigns.GetByIdWithRecipientsAsync(campaignId, cancellationToken);
        if (c == null) return null;
        return new CampaignStatsDto(
            c.Recipients.Count(r => r.Status == RecipientStatus.Sent),
            c.Recipients.Count(r => r.Status == RecipientStatus.Failed),
            c.Recipients.Count(r => r.Status == RecipientStatus.Bounced),
            c.Recipients.Count(r => r.Status == RecipientStatus.Sent),
            c.Recipients.Count(r => r.Status == RecipientStatus.Pending));
    }

    public async Task<IReadOnlyList<CampaignDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _campaigns.GetAllAsync(cancellationToken);
        var result = new List<CampaignDto>();
        foreach (var c in list)
        {
            var withRecipients = await _campaigns.GetByIdWithRecipientsAsync(c.Id, cancellationToken);
            var total = withRecipients?.Recipients.Count ?? 0;
            var sent = withRecipients?.Recipients.Count(r => r.Status == RecipientStatus.Sent) ?? 0;
            var failed = withRecipients?.Recipients.Count(r => r.Status == RecipientStatus.Failed) ?? 0;
            result.Add(new CampaignDto(
                c.Id, c.Name, c.TemplateId, c.Template.Name, c.SenderAccountId, c.SenderAccount.Email,
                c.Status, c.DailyCap, c.DelayMsMin, c.DelayMsMax, c.ScheduledAt, c.StartedAt, c.EndedAt, c.CreatedAt,
                total, sent, failed));
        }
        return result;
    }

    public async Task<CampaignDto> CreateAsync(CreateCampaignCommand cmd, CancellationToken cancellationToken = default)
    {
        var campaign = new Campaign
        {
            Name = cmd.Name,
            TemplateId = cmd.TemplateId,
            SenderAccountId = cmd.SenderAccountId,
            Status = CampaignStatus.Draft,
            DailyCap = cmd.DailyCap,
            DelayMsMin = cmd.DelayMsMin,
            DelayMsMax = cmd.DelayMsMax,
            ScheduledAt = cmd.ScheduledAt
        };
        var added = await _campaigns.AddAsync(campaign, cancellationToken);
        return new CampaignDto(
            added.Id, added.Name, added.TemplateId, "", added.SenderAccountId, "",
            added.Status, added.DailyCap, added.DelayMsMin, added.DelayMsMax, added.ScheduledAt, null, null, added.CreatedAt,
            0, 0, 0);
    }

    /// <summary>Add recipients from contact list(s) and enqueue send jobs. Excludes suppression list.
    /// If senderCaps is provided (e.g. [(sender1Id, 250), (sender2Id, 200)]), distributes recipients across senders: first 250 from sender1, next 200 from sender2, etc.</summary>
    public async Task StartCampaignAsync(int campaignId, IReadOnlyList<int> listIds, IReadOnlyList<(int SenderAccountId, int MaxEmails)>? senderCaps = null, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaigns.GetByIdAsync(campaignId, cancellationToken);
        if (campaign == null) throw new InvalidOperationException("Campaign not found.");
        if (campaign.Status != CampaignStatus.Draft && campaign.Status != CampaignStatus.Paused)
            throw new InvalidOperationException("Campaign must be Draft or Paused to start.");
        var allContactIds = new HashSet<int>();
        foreach (var listId in listIds)
        {
            var list = await _lists.GetByIdAsync(listId, cancellationToken);
            if (list == null) continue;
            foreach (var m in list.Members)
                allContactIds.Add(m.ContactId);
        }
        var contacts = new List<(int ContactId, string Email)>();
        foreach (var cid in allContactIds)
        {
            var c = await _contacts.GetByIdAsync(cid, cancellationToken);
            if (c == null) continue;
            var suppressed = await _suppression.IsSuppressedAsync(c.Email, cancellationToken);
            if (suppressed) continue;
            if (string.IsNullOrWhiteSpace(c.Email)) continue;
            contacts.Add((c.Id, c.Email));
        }
        var campaignRecipients = contacts.Select(t => new CampaignRecipient
        {
            CampaignId = campaignId,
            ContactId = t.ContactId,
            Status = RecipientStatus.Pending
        }).ToList();
        await _recipients.AddRangeAsync(campaignRecipients, cancellationToken);

        int?[]? senderIndexPerJob = null;
        if (senderCaps != null && senderCaps.Count > 0)
        {
            await _campaigns.SetSendersAsync(campaignId, senderCaps, cancellationToken);
            senderIndexPerJob = DistributeSenders(campaignRecipients.Count, senderCaps);
        }

        var jobs = new List<SendJob>();
        for (var i = 0; i < campaignRecipients.Count; i++)
        {
            var r = campaignRecipients[i];
            var job = new SendJob
            {
                CampaignId = campaignId,
                CampaignRecipientId = r.Id,
                Status = SendJobStatus.Pending
            };
            if (senderIndexPerJob != null && senderIndexPerJob[i].HasValue)
                job.SenderAccountId = senderCaps![senderIndexPerJob[i]!.Value].SenderAccountId;
            jobs.Add(job);
        }
        await _sendJobs.AddRangeAsync(jobs, cancellationToken);
        campaign.Status = CampaignStatus.Sending;
        campaign.StartedAt ??= DateTime.UtcNow;
        await _campaigns.UpdateAsync(campaign, cancellationToken);
    }

    /// <summary>Returns for each recipient index the sender index (0-based) to use, or null to use campaign default.</summary>
    private static int?[] DistributeSenders(int recipientCount, IReadOnlyList<(int SenderAccountId, int MaxEmails)> senderCaps)
    {
        var result = new int?[recipientCount];
        var idx = 0;
        for (var s = 0; s < senderCaps.Count && idx < recipientCount; s++)
        {
            var cap = senderCaps[s].MaxEmails;
            for (var k = 0; k < cap && idx < recipientCount; k++)
            {
                result[idx] = s;
                idx++;
            }
        }
        for (; idx < recipientCount; idx++)
            result[idx] = senderCaps.Count - 1;
        return result;
    }

    public async Task PauseCampaignAsync(int campaignId, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaigns.GetByIdAsync(campaignId, cancellationToken);
        if (campaign == null) throw new InvalidOperationException("Campaign not found.");
        campaign.Status = CampaignStatus.Paused;
        await _campaigns.UpdateAsync(campaign, cancellationToken);
    }

    public async Task StopCampaignAsync(int campaignId, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaigns.GetByIdAsync(campaignId, cancellationToken);
        if (campaign == null) throw new InvalidOperationException("Campaign not found.");
        campaign.Status = CampaignStatus.Completed;
        campaign.EndedAt = DateTime.UtcNow;
        await _campaigns.UpdateAsync(campaign, cancellationToken);
    }
}
