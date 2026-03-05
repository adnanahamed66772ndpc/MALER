using MailerApp.Domain.Entities;
using MailerApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailerApp.Infrastructure.Data;

public class SenderAccountRepository : ISenderAccountRepository
{
    private readonly MailerDbContext _db;

    public SenderAccountRepository(MailerDbContext db) => _db = db;

    public async Task<SenderAccount?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.SenderAccounts.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<SenderAccount>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.SenderAccounts.OrderBy(a => a.Name).ToListAsync(cancellationToken);

    public async Task<SenderAccount> AddAsync(SenderAccount account, CancellationToken cancellationToken = default)
    {
        account.CreatedAt = account.UpdatedAt = DateTime.UtcNow;
        _db.SenderAccounts.Add(account);
        await _db.SaveChangesAsync(cancellationToken);
        return account;
    }

    public async Task UpdateAsync(SenderAccount account, CancellationToken cancellationToken = default)
    {
        account.UpdatedAt = DateTime.UtcNow;
        _db.SenderAccounts.Update(account);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.SenderAccounts.FindAsync([id], cancellationToken);
        if (entity != null)
        {
            _db.SenderAccounts.Remove(entity);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
