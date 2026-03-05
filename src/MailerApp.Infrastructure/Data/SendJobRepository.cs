using MailerApp.Domain.Entities;
using MailerApp.Domain.Enums;
using MailerApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailerApp.Infrastructure.Data;

public class SendJobRepository : ISendJobRepository
{
    private readonly MailerDbContext _db;

    public SendJobRepository(MailerDbContext db) => _db = db;

    public async Task<SendJob> AddAsync(SendJob job, CancellationToken cancellationToken = default)
    {
        job.QueuedAt = DateTime.UtcNow;
        _db.SendJobs.Add(job);
        await _db.SaveChangesAsync(cancellationToken);
        return job;
    }

    public async Task AddRangeAsync(IEnumerable<SendJob> jobs, CancellationToken cancellationToken = default)
    {
        var list = jobs.ToList();
        var now = DateTime.UtcNow;
        foreach (var j in list)
            j.QueuedAt = now;
        _db.SendJobs.AddRange(list);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SendJob>> DequeueAsync(int campaignId, int maxCount, CancellationToken cancellationToken = default)
    {
        var jobs = await _db.SendJobs
            .Where(j => j.CampaignId == campaignId && j.Status == SendJobStatus.Pending)
            .OrderBy(j => j.QueuedAt)
            .Take(maxCount)
            .Include(j => j.CampaignRecipient).ThenInclude(r => r.Contact)
            .Include(j => j.Campaign).ThenInclude(c => c.Template)
            .Include(j => j.Campaign).ThenInclude(c => c.SenderAccount)
            .Include(j => j.SenderAccount)
            .ToListAsync(cancellationToken);
        foreach (var j in jobs)
        {
            j.Status = SendJobStatus.Processing;
            j.Attempts++;
        }
        await _db.SaveChangesAsync(cancellationToken);
        return jobs;
    }

    public async Task UpdateAsync(SendJob job, CancellationToken cancellationToken = default)
    {
        if (job.Status == SendJobStatus.Completed || job.Status == SendJobStatus.Failed)
            job.ProcessedAt = DateTime.UtcNow;
        _db.SendJobs.Update(job);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
