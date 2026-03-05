using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MailerApp.Domain.Interfaces;

namespace MailerApp.Application.Campaigns;

public class CampaignExportService
{
    private readonly ICampaignRepository _campaigns;

    public CampaignExportService(ICampaignRepository campaigns) => _campaigns = campaigns;

    public async Task<string> ExportCampaignRecipientsCsvAsync(int campaignId, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaigns.GetByIdWithRecipientsAsync(campaignId, cancellationToken);
        if (campaign == null) throw new InvalidOperationException("Campaign not found.");
        var rows = campaign.Recipients.Select(r => new
        {
            r.Contact.Email,
            r.Contact.FirstName,
            r.Contact.LastName,
            r.Contact.Company,
            Status = r.Status.ToString(),
            r.SentAt,
            r.ErrorCode
        }).ToList();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, config);
        await csv.WriteRecordsAsync(rows, cancellationToken);
        return writer.ToString();
    }
}
