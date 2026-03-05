using MailerApp.Domain.Entities;
using MailerApp.Domain.Enums;
using MailerApp.Domain.Interfaces;

namespace MailerApp.Application.Suppression;

public class SuppressionListService
{
    private readonly ISuppressionListRepository _repo;

    public SuppressionListService(ISuppressionListRepository repo) => _repo = repo;

    public async Task<bool> IsSuppressedAsync(string email, CancellationToken cancellationToken = default) =>
        await _repo.IsSuppressedAsync(email, cancellationToken);

    public async Task AddAsync(string email, SuppressionReason reason, string? source, CancellationToken cancellationToken = default)
    {
        if (await _repo.IsSuppressedAsync(email, cancellationToken)) return;
        await _repo.AddAsync(new SuppressionEntry { Email = email, Reason = reason, Source = source ?? "Manual" }, cancellationToken);
    }

    public async Task<IReadOnlyList<SuppressionEntryDto>> GetAllAsync(int skip = 0, int take = 1000, CancellationToken cancellationToken = default)
    {
        var list = await _repo.GetAllAsync(skip, take, cancellationToken);
        return list.Select(e => new SuppressionEntryDto(e.Id, e.Email, e.Reason, e.Source, e.CreatedAt)).ToList();
    }
}

public record SuppressionEntryDto(int Id, string Email, SuppressionReason Reason, string? Source, DateTime CreatedAt);
