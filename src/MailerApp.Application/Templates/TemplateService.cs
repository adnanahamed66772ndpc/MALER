using System.Text.RegularExpressions;
using MailerApp.Domain.Entities;
using MailerApp.Domain.Interfaces;

namespace MailerApp.Application.Templates;

public class TemplateService
{
    private readonly ITemplateRepository _templates;

    public TemplateService(ITemplateRepository templates) => _templates = templates;

    public async Task<IReadOnlyList<TemplateDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _templates.GetAllAsync(cancellationToken);
        return list.Select(t => new TemplateDto(t.Id, t.Name, t.Subject, t.BodyHtml, t.BodyText, t.CreatedAt, t.UpdatedAt)).ToList();
    }

    public async Task<TemplateDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var t = await _templates.GetByIdAsync(id, cancellationToken);
        return t == null ? null : new TemplateDto(t.Id, t.Name, t.Subject, t.BodyHtml, t.BodyText, t.CreatedAt, t.UpdatedAt);
    }

    public async Task<TemplateDto> CreateAsync(CreateTemplateCommand cmd, CancellationToken cancellationToken = default)
    {
        var t = new Template { Name = cmd.Name, Subject = cmd.Subject, BodyHtml = cmd.BodyHtml, BodyText = cmd.BodyText };
        var added = await _templates.AddAsync(t, cancellationToken);
        return new TemplateDto(added.Id, added.Name, added.Subject, added.BodyHtml, added.BodyText, added.CreatedAt, added.UpdatedAt);
    }

    public async Task<TemplateDto?> UpdateAsync(int id, string name, string subject, string? bodyHtml, string? bodyText, CancellationToken cancellationToken = default)
    {
        var t = await _templates.GetByIdAsync(id, cancellationToken);
        if (t == null) return null;
        t.Name = name;
        t.Subject = subject;
        t.BodyHtml = bodyHtml;
        t.BodyText = bodyText;
        await _templates.UpdateAsync(t, cancellationToken);
        return new TemplateDto(t.Id, t.Name, t.Subject, t.BodyHtml, t.BodyText, t.CreatedAt, t.UpdatedAt);
    }

    /// <summary>Extract {{variableName}} from subject and body.</summary>
    public static IReadOnlyList<string> ExtractVariables(string? subject, string? bodyHtml, string? bodyText)
    {
        var text = $"{subject} {bodyHtml} {bodyText}";
        var matches = Regex.Matches(text, @"\{\{(\w+)\}\}");
        return matches.Select(m => m.Groups[1].Value).Distinct().OrderBy(x => x).ToList();
    }
}
