using MailerApp.Domain.Entities;
using MailerApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailerApp.Infrastructure.Data;

public class SuppressionListRepository : ISuppressionListRepository
{
    private readonly MailerDbContext _db;

    public SuppressionListRepository(MailerDbContext db) => _db = db;

    public async Task<bool> IsSuppressedAsync(string email, CancellationToken cancellationToken = default) =>
        await _db.SuppressionList.AnyAsync(e => e.Email == email, cancellationToken);

    public async Task<SuppressionEntry?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _db.SuppressionList.FirstOrDefaultAsync(e => e.Email == email, cancellationToken);

    public async Task<IReadOnlyList<string>> GetSuppressedEmailsAsync(IEnumerable<string> emails, CancellationToken cancellationToken = default)
    {
        var set = emails.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return await _db.SuppressionList.Where(e => set.Contains(e.Email)).Select(e => e.Email).ToListAsync(cancellationToken);
    }

    public async Task<SuppressionEntry> AddAsync(SuppressionEntry entry, CancellationToken cancellationToken = default)
    {
        entry.CreatedAt = DateTime.UtcNow;
        _db.SuppressionList.Add(entry);
        await _db.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task<IReadOnlyList<SuppressionEntry>> GetAllAsync(int skip = 0, int take = 1000, CancellationToken cancellationToken = default) =>
        await _db.SuppressionList.OrderByDescending(e => e.CreatedAt).Skip(skip).Take(take).ToListAsync(cancellationToken);
}
