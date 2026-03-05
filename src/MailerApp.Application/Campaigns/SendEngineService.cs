using System.Collections.Generic;
using MailerApp.Domain.Entities;
using MailerApp.Domain.Enums;
using MailerApp.Domain.Interfaces;
using Polly;

namespace MailerApp.Application.Campaigns;

public class SendEngineService
{
    private readonly ISendJobRepository _sendJobs;
    private readonly ICampaignRepository _campaigns;
    private readonly ICampaignRecipientRepository _recipients;
    private readonly ISendEventRepository _events;
    private readonly IEmailSenderFactory _senderFactory;

    public SendEngineService(
        ISendJobRepository sendJobs,
        ICampaignRepository campaigns,
        ICampaignRecipientRepository recipients,
        ISendEventRepository events,
        IEmailSenderFactory senderFactory)
    {
        _sendJobs = sendJobs;
        _campaigns = campaigns;
        _recipients = recipients;
        _events = events;
        _senderFactory = senderFactory;
    }

    /// <summary>Process up to maxPerCampaign jobs for each active (Sending) campaign. Applies delay between sends.</summary>
    public async Task ProcessPendingJobsAsync(int maxPerCampaign = 10, CancellationToken cancellationToken = default)
    {
        var active = await _campaigns.GetByStatusAsync(CampaignStatus.Sending, cancellationToken);
        var campaignIds = active.Select(c => c.Id).ToList();
        foreach (var campaignId in campaignIds)
        {
            var jobs = await _sendJobs.DequeueAsync(campaignId, maxPerCampaign, cancellationToken);
            foreach (var job in jobs)
            {
                await ProcessOneJobAsync(job, cancellationToken);
                await Task.Delay(Random.Shared.Next(job.Campaign.DelayMsMin, Math.Max(job.Campaign.DelayMsMin, job.Campaign.DelayMsMax) + 1), cancellationToken);
            }
        }
    }

    private async Task ProcessOneJobAsync(SendJob job, CancellationToken cancellationToken)
    {
        var recipient = job.CampaignRecipient;
        var contact = recipient.Contact;
        var campaign = job.Campaign;
        var template = campaign.Template;
        var senderAccount = job.SenderAccount ?? campaign.SenderAccount;
        var sender = await _senderFactory.CreateForAccountAsync(senderAccount, cancellationToken);
        if (sender == null) { await MarkFailedAsync(job, recipient, "No sender", cancellationToken); return; }
        var subject = ReplaceVariables(template.Subject, contact);
        var bodyHtml = ReplaceVariables(template.BodyHtml ?? "", contact);
        var bodyText = ReplaceVariables(template.BodyText ?? "", contact);
        var policy = Policy.Handle<Exception>(e => IsTransient(e))
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
        try
        {
            await policy.ExecuteAsync(async () =>
                await sender.SendAsync(senderAccount.Email, contact.Email, subject, bodyHtml, bodyText, cancellationToken));
            recipient.Status = RecipientStatus.Sent;
            recipient.SentAt = DateTime.UtcNow;
            job.Status = SendJobStatus.Completed;
            await _recipients.UpdateAsync(recipient, cancellationToken);
            await _sendJobs.UpdateAsync(job, cancellationToken);
            await _events.AddAsync(new SendEvent { SendJobId = job.Id, CampaignRecipientId = recipient.Id, EventType = SendEventType.Sent }, cancellationToken);
        }
        catch (Exception ex)
        {
            await MarkFailedAsync(job, recipient, ex.Message, cancellationToken);
        }
    }

    private async Task MarkFailedAsync(SendJob job, CampaignRecipient recipient, string error, CancellationToken cancellationToken)
    {
        recipient.Status = RecipientStatus.Failed;
        recipient.ErrorCode = error.Length > 500 ? error[..500] : error;
        job.Status = SendJobStatus.Failed;
        await _recipients.UpdateAsync(recipient, cancellationToken);
        await _sendJobs.UpdateAsync(job, cancellationToken);
        await _events.AddAsync(new SendEvent { SendJobId = job.Id, CampaignRecipientId = recipient.Id, EventType = SendEventType.Bounce, PayloadJson = "{\"error\":\"" + error.Replace("\"", "'") + "\"}" }, cancellationToken);
    }

    private static string ReplaceVariables(string text, Contact c)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["firstName"] = c.FirstName ?? "",
            ["lastName"] = c.LastName ?? "",
            ["company"] = c.Company ?? "",
            ["email"] = c.Email ?? ""
        };
        foreach (var (k, v) in map)
            text = text.Replace("{{" + k + "}}", v);
        return text;
    }

    private static bool IsTransient(Exception ex)
    {
        var msg = ex.Message.ToLowerInvariant();
        return msg.Contains("timeout") || msg.Contains("connection") || msg.Contains("network") || ex is OperationCanceledException;
    }
}
