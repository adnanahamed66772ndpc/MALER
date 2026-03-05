using MailerApp.Domain.Entities;
using MailerApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailerApp.Infrastructure.Data;

public class TemplateRepository : ITemplateRepository
{
    private readonly MailerDbContext _db;

    public TemplateRepository(MailerDbContext db) => _db = db;

    public async Task<Template?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.Templates.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<Template>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.Templates.OrderBy(t => t.Name).ToListAsync(cancellationToken);

    public async Task<Template> AddAsync(Template template, CancellationToken cancellationToken = default)
    {
        template.CreatedAt = template.UpdatedAt = DateTime.UtcNow;
        _db.Templates.Add(template);
        await _db.SaveChangesAsync(cancellationToken);
        return template;
    }

    public async Task UpdateAsync(Template template, CancellationToken cancellationToken = default)
    {
        template.UpdatedAt = DateTime.UtcNow;
        _db.Templates.Update(template);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
