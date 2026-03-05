using MailerApp.Domain.Entities;

namespace MailerApp.Domain.Interfaces;

public interface ITemplateRepository
{
    Task<Template?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Template>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Template> AddAsync(Template template, CancellationToken cancellationToken = default);
    Task UpdateAsync(Template template, CancellationToken cancellationToken = default);
}
